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
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        [Route(RouteConstants.SEARCH_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult FullText(FullTextDTO model)
        {
			if (string.IsNullOrWhiteSpace(model?.SearchQuery)) return BadRequest("A search query is required");

			var searchResult = UCommerce.Api.SearchLibrary.GetProductsByName(model.SearchQuery);

			return Ok(this.ConvertToFullTextSearchResultModel(searchResult, model.ProductDetailsPageId));
        }

        [Route(RouteConstants.SEARCH_SUGGESTIONS_ROUTE_VALUE)]
        [HttpPost]
        public IHttpActionResult Suggestions(FullTextDTO model)
        {
			if (string.IsNullOrWhiteSpace(model?.SearchQuery)) return BadRequest("A search query is required");

			var searchResult = UCommerce.Api.SearchLibrary.GetProductNameSuggestions(model.SearchQuery);
            // TODO: suggestion searching not supported in Ucommerce v9.0
            //var searchResult = Ucommerce.Api.SearchLibrary.GetProductNameSuggestions(model.SearchQuery);

			return Ok(searchResult);
        }

        private IList<FullTextSearchResultDTO> ConvertToFullTextSearchResultModel(ResultSet<Product> products, Guid? productDetailsPageId)
        {
            var fullTextSearchResultModels = new List<FullTextSearchResultDTO>();

			if (products == null || !products.Any()) return fullTextSearchResultModels;

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

                    detailsPageUrl += catUrl + "/" + product.Slug;
                    detailsPageUrl = Pages.UrlResolver.GetAbsoluteUrl(detailsPageUrl);
                }

                if (string.IsNullOrWhiteSpace(detailsPageUrl))
                {
                    //var entityProduct = this.productRepository.Get(product.Id);
                    detailsPageUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
					var entityProduct = this.productRepository.Get(product.Id);
					detailsPageUrl = CatalogLibrary.GetNiceUrlForProduct(entityProduct);
				}

				var price = string.Empty;
				if (productsPrices?.FirstOrDefault(x => x.ProductGuid == product.Guid)?.PriceInclTax != null)
				{
					price = new Money(productsPrices.First(x => x.ProductGuid == product.Guid).PriceInclTax, currency).ToString();
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
