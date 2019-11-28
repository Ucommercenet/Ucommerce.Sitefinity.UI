using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Constants;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Search;
using Product = UCommerce.Documents.Product;

namespace UCommerce.Sitefinity.UI.Api
{
    /// <summary>
    /// API Controller exposing endpoints related to search.
    /// </summary>
    public class SearchApiController : ApiController
    {
        private readonly IRepository<UCommerce.EntitiesV2.Product> productRepository;

        public SearchApiController()
        {
            this.productRepository = ObjectFactory.Instance.Resolve<IRepository<UCommerce.EntitiesV2.Product>>();
        }

        [Route(RouteConstants.SEARCH_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult FullText(FullTextDTO model)
        {
            var searchResult = UCommerce.Api.SearchLibrary.GetProductsByName(model.SearchQuery);

            return Ok(this.ConvertToFullTextSearchResultModel(searchResult, model.ProductDetailsPageId));
        }

        [Route(RouteConstants.SEARCH_SUGGESTIONS_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult Suggestions(FullTextDTO model)
        {
            var searchResult = UCommerce.Api.SearchLibrary.GetProductNameSuggestions(model.SearchQuery);

            return Ok(searchResult);
        }

        private IList<FullTextSearchResultDTO> ConvertToFullTextSearchResultModel(IList<Product> products, Guid? productDetailsPageId)
        {
            var fullTextSearchResultModels = new List<FullTextSearchResultDTO>();

            var currency = UCommerce.Runtime.SiteContext.Current.CatalogContext.CurrentPriceGroup.Currency;
            var productsPrices = UCommerce.Api.CatalogLibrary.CalculatePrice(products.Select(x => x.Guid).ToList()).Items;
            ProductCatalog catalog = UCommerce.Api.CatalogLibrary.GetCatalog();



            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            foreach (var product in products)
            {
                string catUrl =  CategoryModel.DefaultCategoryName;

                if (product.CategoryIds != null && product.CategoryIds.Any())
                {
                    var productCategory = catalog.Categories
                                                 .Where(c => c.CategoryId == product.CategoryIds.FirstOrDefault())
                                                 .FirstOrDefault();

                    if (productCategory != null)
                    {
                        catUrl = CategoryModel.GetCategoryPath(productCategory);
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

                    detailsPageUrl += catUrl + "/" + product.Id.ToString();
                    detailsPageUrl = Pages.UrlResolver.GetAbsoluteUrl(detailsPageUrl);
                }

                if (string.IsNullOrWhiteSpace(detailsPageUrl))
                {
                    var entityProduct = this.productRepository.Get(product.Id);
                    detailsPageUrl = CatalogLibrary.GetNiceUrlForProduct(entityProduct);
                }

                var fullTestSearchResultModel = new FullTextSearchResultDTO()
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
