using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Controllers;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Search;
using UCommerce.Sitefinity.UI.Search;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Extensions;
using System.Web;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Facet Filter MVC widget.
    /// </summary>
    public class FacetsFilterModel : IFacetsFilterModel
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();

        public virtual IList<FacetViewModel> CreateViewModel()
        {
            var pageContext = SystemManager.CurrentHttpContext.GetProductsContext();
            var categoryIds = pageContext.GetValue<ProductsController>(p => p.CategoryIds);
            var productIds = pageContext.GetValue<ProductsController>(p => p.ProductIds);
            if (!string.IsNullOrEmpty(categoryIds) || !string.IsNullOrEmpty(productIds))
            {
                return this.MapFacetsByManualSelection(categoryIds, productIds);
            }

            Category currentCategory = null;

            if (CatalogContext.CurrentCategory != null)
            {
                currentCategory = Category.FirstOrDefault(c => c.Name == CatalogContext.CurrentCategory.Name);
            }

            return this.GetAllFacets(currentCategory);
        }

        public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            message = null;
            return true;
        }

        private IList<FacetViewModel> GetAllFacets(Category category)
        {
            var facets = HttpContext.Current.Request.QueryString.ToFacets();
            IList<Ucommerce.Search.Facets.Facet> allFacets;

            if (category != null)
            {
                allFacets = CatalogLibrary.GetFacets(category.Guid, facets.ToFacetDictionary());
            }
            else
            {
                allFacets = CatalogLibrary.GetFacets(CatalogContext.CurrentCatalog.Categories, facets.ToFacetDictionary());
            }

            return this.MapToFacetsViewModel(allFacets);
        }

        private IList<FacetViewModel> MapFacetsByManualSelection(string categoryIdsString, string productIdsString)
        {
            var categoryIds = categoryIdsString?.Split(',').Select(x => Convert.ToInt32(x)).ToList() ?? new List<int>();
            var productIds = productIdsString?.Split(',').Select(x => Convert.ToInt32(x)).ToList() ?? new List<int>();
            var facets = HttpContext.Current.Request.QueryString.ToFacets();
            var categories = Category.Find(x => categoryIds.Any(y => y == x.Id));

            return this.MapToFacetsViewModel(
                CatalogLibrary.GetFacets(categories.Select(x => x.Guid).ToList(), facets.ToFacetDictionary()));
        }

        private IList<FacetViewModel> MapToFacetsViewModel(IList<Ucommerce.Search.Facets.Facet> facets)
        {
            return facets.Select(x => new FacetViewModel()
            {
                DisplayName = x.DisplayName,
                Name = x.Name,
                FacetValues = x.FacetValues
                    .Select(y => new FacetValue(y.Value, (int) y.Count))
                    .ToList(),
            }).ToList();
        }

        private readonly IList<string> queryStringBlackList = new List<string>() { "product", "category", "catalog" };
    }
}
