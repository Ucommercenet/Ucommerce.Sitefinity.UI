using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;
using Telerik.Sitefinity.Services;
using Ucommerce.Sitefinity.UI.Mvc.Controllers;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce.Sitefinity.UI.Pages;
using Ucommerce.Sitefinity.UI.Search;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;
using UCommerce.Search;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class FacetsFilterModel : IFacetsFilterModel
    {
        public IList<FacetViewModel> CreateViewModel()
        {
            var pageContext = SystemManager.CurrentHttpContext.GetProductsContext();
            var categoryIds = pageContext.GetValue<ProductsController>(p => p.CategoryIds);
            var productIds = pageContext.GetValue<ProductsController>(p => p.ProductIds);
            if (!string.IsNullOrEmpty(categoryIds) || !string.IsNullOrEmpty(productIds))
            {
                return this.MapFacetsByManualSelection(categoryIds, productIds);
            }

            Category currentCategory = null;

            if (SiteContext.Current.CatalogContext.CurrentCategory != null)
            {
                currentCategory = Category.FirstOrDefault(c => c.Name == SiteContext.Current.CatalogContext.CurrentCategory.Name);
            }

            return this.GetAllFacets(currentCategory);
        }

        private IList<FacetViewModel> GetAllFacets(Category category)
        {
            var facetsResolver = new FacetResolver(this.queryStringBlackList);
            var facetsForQuerying = facetsResolver.GetFacetsFromQueryString();
            IList<UCommerce.Search.Facets.Facet> allFacets;

            if (category != null)
            {
                allFacets = SearchLibrary.GetFacetsFor(category, facetsForQuerying);
            }
            else
            {
                allFacets = SearchLibrary.FacetedQuery()
                      .WithFacets(facetsForQuerying)
                      .ToFacets()
                      .ToList();
            }

            return this.MapToFacetsViewModel(allFacets);
        }

        private IList<FacetViewModel> MapFacetsByManualSelection(string categoryIdsString, string productIdsString)
        {
            var categoryIds = categoryIdsString?.Split(',').Select(x => Convert.ToInt32(x)).ToList() ?? new List<int>();
            var productIds = productIdsString?.Split(',').Select(x => Convert.ToInt32(x)).ToList() ?? new List<int>();

            var facetsResolver = new FacetResolver(this.queryStringBlackList);
            var facetsForQuerying = facetsResolver.GetFacetsFromQueryString();
            return this.MapToFacetsViewModel(SearchLibrary.FacetedQuery()
                .Where(x => x.CategoryIds.In(categoryIds) || x.Id.In(productIds)).WithFacets(facetsForQuerying).ToFacets().ToList());
        }

        private IList<FacetViewModel> MapToFacetsViewModel(IList<UCommerce.Search.Facets.Facet> facets)
        {
            return facets.Select(x => new FacetViewModel()
            {
                DisplayName = x.DisplayName,
                Name = x.Name,
                FacetValues = x.FacetValues
                    .Select(y => new FacetValue(y.Value, y.Hits))
                    .ToList(),
            }).ToList();
        }

        private readonly IList<string> queryStringBlackList = new List<string>() { "product", "category", "catalog" };
    }
}
