using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Sitefinity.UI.Api.Model;
using UCommerce;
using UCommerce.Api;
using UCommerce.Catalog.Models;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Runtime;

namespace Ucommerce.Sitefinity.UI.Api
{
    [RoutePrefix("ProductApi")]
    public class ProductController : ApiController
    {
        [Route("productPrices")]
        [HttpPost]
        public IHttpActionResult ProductPrices(ProductPricesModel productPricesModel)
        {
            var currentPriceGroupCurrency = SiteContext.Current.CatalogContext.CurrentPriceGroup.Currency;
            var productGuidsToGetPricesFor = ProductGuidsToGetPricesFor(productPricesModel);

            var productsPrices = CatalogLibrary.CalculatePrice(productGuidsToGetPricesFor);

            return Json(MapToProductPricesViewModels(productsPrices, currentPriceGroupCurrency));
        }

        [Route("productReviews")]
        [HttpPost]
        public IHttpActionResult GetProductReviews([FromBody] GetProductReviewsModel model)
        {
            var productReviews = Product.FirstOrDefault(x => x.Sku == model.Sku && x.VariantSku == null).ProductReviews
                .Where(x => x.ProductReviewStatus.ProductReviewStatusId == 2000);

            return Json(productReviews.Select(x => new ProductReviewModel()
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
        public IHttpActionResult PutProductReview([FromBody] ProductReviewModel model)
        {
            var product = Product.FirstOrDefault(x => x.Sku == model.Sku && x.VariantSku == null);
            var request = System.Web.HttpContext.Current.Request;
            var basket = TransactionLibrary.HasBasket() ? TransactionLibrary.GetBasket(false) : null;

            if (basket != null)
            {
                if (basket.PurchaseOrder.Customer == null)
                {
                    basket.PurchaseOrder.Customer = new Customer() { FirstName = model.Name, LastName = string.Empty, EmailAddress = model.Email };
                    basket.Save();
                }
            }

            var review = new ProductReview()
            {
                ProductCatalogGroup = SiteContext.Current.CatalogContext.CurrentCatalogGroup,
                ProductReviewStatus = ProductReviewStatus.SingleOrDefault(s => s.Name == "New"),
                CreatedOn = DateTime.Now,
                CreatedBy = model.Name,
                Product = product,
                Customer = basket?.PurchaseOrder?.Customer,
                Rating = model.Rating,
                ReviewHeadline = model.Title,
                ReviewText = model.Comments,
                Ip = request.UserHostName,
            };

            product.AddProductReview(review);

            PipelineFactory.Create<ProductReview>("ProductReview").Execute(review);

            return Ok();
        }

        private IList<ProductPricesViewModel> MapToProductPricesViewModels(ProductPriceCalculationResult productsPrices, Currency currency)
        {
            var prices = new List<ProductPricesViewModel>();

            foreach (var productsPricesItem in productsPrices.Items)
            {
                var productPricesViewModel = new ProductPricesViewModel();

                productPricesViewModel.ProductGuid = productsPricesItem.ProductGuid;
                productPricesViewModel.Price = new Money(productsPricesItem.PriceInclTax, currency).ToString();
                productPricesViewModel.ListPrice = new Money(productsPricesItem.ListPriceInclTax, currency).ToString();

                prices.Add(productPricesViewModel);
            }

            return prices;
        }

        private List<Guid> ProductGuidsToGetPricesFor(ProductPricesModel productPricesModel)
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