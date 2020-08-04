using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Constants;
using UCommerce.Sitefinity.UI.Mvc.Model;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Product = Ucommerce.Search.Models.Product; 
using Ucommerce.Search.Slugs;

namespace UCommerce.Sitefinity.UI.Api
{
    /// <summary>
    /// API Controller exposing endpoints related to search.
    /// </summary>
    public class SearchApiController : ApiController
    {
        public IIndex<Ucommerce.Search.Models.Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Ucommerce.Search.Models.Product>>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        [Route(RouteConstants.SEARCH_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult FullText(FullTextDTO model)
        {
            var products = ProductIndex.Find()
                .Where(p => p.Name.Contains(model.SearchQuery) || p.DisplayName == Match.FullText(model.SearchQuery))
                .ToList();

            return Ok(ConvertToFullTextSearchResultModel(products, model.ProductDetailsPageId));
        }

        [Route(RouteConstants.SEARCH_SUGGESTIONS_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult Suggestions(FullTextDTO model)
        {
            // TODO: sugestion searching not supported in Ucommerce v9.0
            //var searchResult = Ucommerce.Api.SearchLibrary.GetProductNameSuggestions(model.SearchQuery);

            return FullText(model);
        }

        private IList<FullTextSearchResultDTO> ConvertToFullTextSearchResultModel(ResultSet<Product> products, Guid? productDetailsPageId)
        {
            var fullTextSearchResultModels = new List<FullTextSearchResultDTO>();

            var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            var productsPrices = CatalogLibrary.CalculatePrices(products.Select(x => x.Guid).ToList()).Items;
            var catalog = CatalogLibrary.GetCatalog(products.First().Guid);

            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            foreach (var product in products)
            {
                string catUrl =  CategoryModel.DefaultCategoryName;

                if (product.Categories != null && product.Categories.Any())
                {
                    var productCategoryId = catalog.Categories
                                                 .Where(x => x == product.Categories.FirstOrDefault())
                                                 .FirstOrDefault();

                    if (productCategoryId != null)
                    {
                        var category = CatalogContext.CurrentCategories.FirstOrDefault(x => x.Guid == productCategoryId);
                        catUrl = CategoryModel.GetCategoryPath(category);
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

                    detailsPageUrl += catUrl + "/" + product.Guid.ToString();
                    detailsPageUrl = Pages.UrlResolver.GetAbsoluteUrl(detailsPageUrl);
                }

                if (string.IsNullOrWhiteSpace(detailsPageUrl))
                {
                    //var entityProduct = this.productRepository.Get(product.Id);
                    detailsPageUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
                }

                var fullTestSearchResultModel = new FullTextSearchResultDTO()
                {
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    Name = product.Name,
                    Url = detailsPageUrl,
                    Price = new Money(productsPrices.First(x => x.ProductGuid == product.Guid).PriceInclTax, currencyIsoCode).ToString(),
                };

                fullTextSearchResultModels.Add(fullTestSearchResultModel);
            }

            return fullTextSearchResultModels;
        }
    }
}
