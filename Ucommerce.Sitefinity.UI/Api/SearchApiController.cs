using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Sitefinity.UI.Api.Model;
using Ucommerce.Sitefinity.UI.Constants;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Search;
using Product = UCommerce.Documents.Product;

namespace Ucommerce.Sitefinity.UI.Api
{
    public class SearchApiController : ApiController
    {
        private readonly IRepository<UCommerce.EntitiesV2.Product> productRepository;

        public SearchApiController()
        {
            this.productRepository = ObjectFactory.Instance.Resolve<IRepository<UCommerce.EntitiesV2.Product>>();
        }

        [Route(RouteConstants.SEARCH_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult FullText(FullTextModel model)
        {
            var searchResult = UCommerce.Api.SearchLibrary.GetProductsByName(model.SearchQuery);

            return Ok(this.ConvertToFullTextSearchResultModel(searchResult, model.ProductDetailsPageId));
        }

        [Route(RouteConstants.SEARCH_SUGGESTIONS_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult Suggestions(FullTextModel model)
        {
            var searchResult = UCommerce.Api.SearchLibrary.GetProductNameSuggestions(model.SearchQuery);

            return Ok(searchResult);
        }

        private IList<FullTextSearchResultModel> ConvertToFullTextSearchResultModel(IList<Product> products, Guid? productDetailsPageId)
        {
            var fullTextSearchResultModels = new List<FullTextSearchResultModel>();

            var currency = UCommerce.Runtime.SiteContext.Current.CatalogContext.CurrentPriceGroup.Currency;
            var productsPrices = UCommerce.Api.CatalogLibrary.CalculatePrice(products.Select(x => x.Guid).ToList()).Items;
            ProductCatalog catalog = UCommerce.Api.CatalogLibrary.GetCatalog();



            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            foreach (var product in products)
            {
                string categoryName = "products";

                if (product.CategoryIds != null && product.CategoryIds.Any())
                {
                    var productCategory = catalog.Categories
                                                 .Where(c => c.CategoryId == product.CategoryIds.FirstOrDefault())
                                                 .FirstOrDefault();

                    if (productCategory != null)
                    {
                        categoryName = productCategory.Name;
                    }
                }

                string detailsPageUrl = string.Empty;

                if (productDetailsPageId != null && productDetailsPageId.Value != Guid.Empty)
                {
                    detailsPageUrl = Pages.UrlResolver.GetPageNodeUrl(productDetailsPageId.Value);

                    if (!detailsPageUrl.EndsWith("/"))
                    {
                        detailsPageUrl += "/";
                    }

                    detailsPageUrl += categoryName + "/" + product.Id.ToString();
                    detailsPageUrl = Pages.UrlResolver.GetAbsoluteUrl(detailsPageUrl);

                }

                if (string.IsNullOrWhiteSpace(detailsPageUrl))
                {
                    var entityProduct = this.productRepository.Get(product.Id);
                    detailsPageUrl = CatalogLibrary.GetNiceUrlForProduct(entityProduct);
                }

                var fullTestSearchResultModel = new FullTextSearchResultModel()
                {
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    Name = product.Name,
                    Url = detailsPageUrl,
                    Price = new Money(productsPrices.First(x => x.ProductGuid == product.Guid).PriceInclTax, currency).ToString(),
                };

                fullTextSearchResultModels.Add(fullTestSearchResultModel);
            }

            return fullTextSearchResultModels;
        }
    }
}
