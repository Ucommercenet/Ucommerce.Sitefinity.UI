using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Services;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Extensions;
using Ucommerce.Search.Facets;
using UCommerce.Sitefinity.UI.Mvc.Controllers;
using UCommerce.Sitefinity.UI.Mvc.Services;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using UCommerce.Sitefinity.UI.Search;
using FacetValue = UCommerce.Sitefinity.UI.Mvc.ViewModels.FacetValue;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Facet Filter MVC widget.
    /// </summary>
    public class FacetsFilterModel : IFacetsFilterModel
    {
        private readonly IList<string> queryStringBlackList = new List<string>
            { "product", "category", "catalog" };

        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();

        public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            message = null;
            return true;
        }

        // TODO: Check if we're manually sorting the products (as we do on the product listing page)
        public virtual IList<FacetViewModel> CreateViewModel()
        {
            Category currentCategory = null;

            if (CatalogContext.CurrentCategory != null)
            {
                currentCategory = Category.FirstOrDefault(c => c.Name == CatalogContext.CurrentCategory.Name);
                return GetAllFacets(currentCategory);
            }

            var pageContext = SystemManager.CurrentHttpContext.GetProductsContext();
            var categoryIds = pageContext.GetValue<ProductsController>(p => p.CategoryIds);
            var productIds = pageContext.GetValue<ProductsController>(p => p.ProductIds);
            if (!string.IsNullOrEmpty(categoryIds) || !string.IsNullOrEmpty(productIds))
            {
                return MapFacetsByManualSelection(categoryIds, productIds);
            }

            return GetAllFacets(currentCategory);
        }

        private IList<FacetViewModel> GetAllFacets(Category category)
        {
            var facets = HttpContext.Current.Request.QueryString.ToFacets();
            IList<Facet> allFacets;

            if (category != null)
            {
                allFacets = CatalogLibrary.GetFacets(category.Guid, facets.ToFacetDictionary());
            }
            else
            {
                allFacets = CatalogLibrary.GetFacets(CatalogContext.CurrentCatalog.Categories, facets.ToFacetDictionary());
            }

            return MapToFacetsViewModel(allFacets);
        }

        private IList<FacetViewModel> MapFacetsByManualSelection(string categoryIdsString, string productIdsString)
        {
            var categoryIds = categoryIdsString?.Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToList() ?? new List<int>();
            var facets = HttpContext.Current.Request.QueryString.ToFacets();
            var categories = Category.Find(x => categoryIds.Contains(x.CategoryId));

            return MapToFacetsViewModel(
                CatalogLibrary.GetFacets(categories.Select(x => x.Guid)
                        .ToList(),
                    facets.ToFacetDictionary()));
        }

        private IList<FacetViewModel> MapToFacetsViewModel(IList<Facet> facets)
        {
            var facetViewModel = facets.Select(x => new FacetViewModel
            {
                DisplayName = x.DisplayName,
                Name = x.Name,
                FacetValues = x.FacetValues
                    .Select(y => new FacetValue(y.Value, (int)y.Count))
                    .ToList(),
            });

            var currentPriceGroup = PriceGroup.FirstOrDefault(x => x.Guid == CatalogContext.CurrentPriceGroup.Guid);

            return (from f in facetViewModel
                    where f.FacetValues.Any()
                    let name = f.Name.ToLower()
                    let isPriceFilter = name.Contains("price")
                    where !isPriceFilter || (isPriceFilter && name.Contains("price") && name.EndsWith(currentPriceGroup.Name.ToLower(), StringComparison.Ordinal))
                    select f
                ).ToList();
        }
    }
}
