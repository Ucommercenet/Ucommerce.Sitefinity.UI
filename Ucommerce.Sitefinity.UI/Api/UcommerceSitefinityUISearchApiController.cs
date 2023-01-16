using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;
using UCommerce.Sitefinity.UI.Api.Model;
using UCommerce.Sitefinity.UI.Constants;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Pages;

namespace UCommerce.Sitefinity.UI.Api
{
    /// <summary>
    /// API Controller exposing endpoints related to search.
    /// </summary>
    public class UcommerceSitefinityUISearchApiController : ApiController
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        [Route(RouteConstants.SEARCH_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult FullText(FullTextDTO model)
        {
            if (string.IsNullOrWhiteSpace(model?.SearchQuery))
            {
                return BadRequest("A search query is required");
            }

            var searchResult = ProductIndex
                .Find<Product>()
                .Where(x =>
                    x.Name == Match.Fuzzy(model.SearchQuery, 1)
                    || x.DisplayName == Match.Fuzzy(model.SearchQuery, 1)
                    || x.Name.Contains(model.SearchQuery)
                    || x.DisplayName.Contains(model.SearchQuery)
                )
                .ToList();
            return Ok(ConvertToFullTextSearchResultModel(searchResult, model.ProductDetailsPageId));
        }

        [Route(RouteConstants.SEARCH_SUGGESTIONS_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult Suggestions(FullTextDTO model)
        {
            if (string.IsNullOrWhiteSpace(model?.SearchQuery))
            {
                return BadRequest("A search query is required");
            }

            // NOTE: Unfortunately Suggestions have been removed in v 9.0 in favor of a faster release date. This is on the road map and will arrive in a later version.
            // See: https://docs.ucommerce.net/ucommerce/v9.2/Migration/Migrating-to-v9/Migrate-Search-Library.html#suggestions
            // var searchResult = Ucommerce.Api.SearchLibrary.GetProductNameSuggestions(model.SearchQuery);
            // return Ok(searchResult);
            return Ok();
        }

        private IList<FullTextSearchResultDTO> ConvertToFullTextSearchResultModel(ResultSet<Product> products, Guid? productDetailsPageId)
        {
            var fullTextSearchResultModels = new List<FullTextSearchResultDTO>();

            if (products == null || !products.Any())
            {
                return fullTextSearchResultModels;
            }

            var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            var productsPrices = CatalogLibrary.CalculatePrices(products.Select(x => x.Guid)
                    .ToList())
                .Items;
            var catalog = CatalogContext.CurrentCatalog;

            foreach (var product in products)
            {
                var catUrl = string.Empty;

                if (product.Categories != null && product.Categories.Any())
                {
                    var productCategoryId = catalog.Categories.FirstOrDefault(x => product.Categories.Any(c => c == x));
                    if (productCategoryId != Guid.Empty)
                    {
                        var category = CatalogContext.CurrentCategories.FirstOrDefault(x => x.Guid == productCategoryId);
                        catUrl = CategoryModel.GetCategoryPath(category);
                    }
                }

                if (string.IsNullOrWhiteSpace(catUrl))
                {
                    catUrl = CategoryModel.DefaultCategoryName;
                }

                var detailsPageUrl = string.Empty;
                if (productDetailsPageId != null && productDetailsPageId.Value != Guid.Empty)
                {
                    detailsPageUrl = UrlResolver.GetPageNodeUrl(productDetailsPageId.Value);

                    if (!detailsPageUrl.EndsWith("/", StringComparison.Ordinal))
                    {
                        detailsPageUrl += "/";
                    }

                    detailsPageUrl += $"{catUrl}/p/{product.Slug}";
                    detailsPageUrl = UrlResolver.GetAbsoluteUrl(detailsPageUrl);
                }

                if (string.IsNullOrWhiteSpace(detailsPageUrl))
                {
                    detailsPageUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
                }

                var price = string.Empty;
                if (productsPrices?.FirstOrDefault(x => x.ProductGuid == product.Guid)
                        ?.PriceInclTax != null)
                {
                    price = new Money(productsPrices.First(x => x.ProductGuid == product.Guid)
                            .PriceInclTax,
                        currencyIsoCode).ToString();
                }

                var fullTestSearchResultModel = new FullTextSearchResultDTO
                {
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    Name = product.Name,
                    Url = detailsPageUrl,
                    Price = new Money(productsPrices.First(x => x.ProductGuid == product.Guid)
                            .PriceInclTax,
                        currencyIsoCode).ToString(),
                };

                fullTextSearchResultModels.Add(fullTestSearchResultModel);
            }

            return fullTextSearchResultModels;
        }
    }
}
