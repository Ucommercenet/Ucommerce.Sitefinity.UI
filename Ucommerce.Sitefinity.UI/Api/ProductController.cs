using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using UCommerce.Sitefinity.UI.Api.Model;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Catalog.Models;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;
using Ucommerce.Infrastructure;

namespace UCommerce.Sitefinity.UI.Api
{
    /// <summary>
    /// API Controller exposing endpoints for working with <see cref="Product"/> items.
    /// </summary>
    [RoutePrefix("ProductApi")]
    public class ProductController : ApiController
    {
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [Route("productPrices")]
        [HttpPost]
        public IHttpActionResult ProductPrices(ProductDTO productPricesModel)
        {
            var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            var productGuidsToGetPricesFor = ProductGuidsToGetPricesFor(productPricesModel);
            var productsPrices = CatalogLibrary.CalculatePrices(productGuidsToGetPricesFor);

            return Json(MapToProductPricesViewModels(productsPrices, currencyIsoCode));
        }

        [Route("productReviews")]
        [HttpPost]
        public IHttpActionResult GetProductReviews([FromBody] GetProductReviewsDTO model)
        {
            var productReviews = Product.FirstOrDefault(x => x.Sku == model.Sku && x.VariantSku == null).ProductReviews
                .Where(x => x.ProductReviewStatus.ProductReviewStatusId == 2000);

            return Json(productReviews.Select(x => new ProductReviewDTO()
            {
                Name = x.CreatedBy,
                Rating = x.Rating.GetValueOrDefault(),
                Comments = x.ReviewText,
                Title = x.ReviewHeadline,
                Submitted = x.CreatedOn,
            }));
        }

        [Route("productReviews/put")]
        [HttpPost]
        public IHttpActionResult PutProductReview([FromBody] ProductReviewDTO model)
        {
            var product = Product.FirstOrDefault(x => x.Sku == model.Sku && x.VariantSku == null);
            var productCatalogGroup = ProductCatalogGroup
                .FirstOrDefault(x => x.ProductCatalogGroupId == CatalogContext.CurrentCatalogGroup.ProductCatalogGroupId);
            var request = System.Web.HttpContext.Current.Request;
            var basket = TransactionLibrary.HasBasket() ? TransactionLibrary.GetBasket(false) : null;

            if (basket != null)
            {
                if (basket.Customer == null)
                {
                    basket.Customer = new Customer() { FirstName = model.Name, LastName = string.Empty, EmailAddress = model.Email };
                    basket.Save();
                }
            }

            var review = new ProductReview()
            {
                ProductCatalogGroup = productCatalogGroup,
                ProductReviewStatus = ProductReviewStatus.SingleOrDefault(s => s.Name == "New"),
                CreatedOn = DateTime.Now,
                CreatedBy = model.Name,
                Product = product,
                Customer = basket?.Customer,
                Rating = model.Rating,
                ReviewHeadline = model.Title,
                ReviewText = model.Comments,
                Ip = request.UserHostName,
            };

            product.AddProductReview(review);

            PipelineFactory.Create<ProductReview>("ProductReview").Execute(review);

            return Ok();
        }

        private IList<ProductPricesDTO> MapToProductPricesViewModels(ProductPriceCalculationResult productsPrices, string currencyIsoCode)
        {
            var prices = new List<ProductPricesDTO>();

            foreach (var productsPricesItem in productsPrices.Items)
            {
                var productPricesViewModel = new ProductPricesDTO
                {
                    ProductGuid = productsPricesItem.ProductGuid,
                    Price = new Money(productsPricesItem.PriceInclTax, currencyIsoCode).ToString(),
                    ListPrice = new Money(productsPricesItem.ListPriceInclTax, currencyIsoCode).ToString()
                };

                prices.Add(productPricesViewModel);
            }

            return prices;
        }

        private List<Guid> ProductGuidsToGetPricesFor(ProductDTO productPricesModel)
        {
            var productGuidsToGetPricesFor = new List<Guid>();

            if (productPricesModel.ProductGuid != Guid.Empty)
            {
                productGuidsToGetPricesFor.Add(productPricesModel.ProductGuid);
            }

            if (productPricesModel.VariantGuid != Guid.Empty)
            {
                productGuidsToGetPricesFor.Add(productPricesModel.VariantGuid);
            }

            return productGuidsToGetPricesFor;
        }
    }
}