using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Controllers;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using Ucommerce.Api;
using UCommerce.Sitefinity.UI.Search;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Extensions;
using System.Web;
using UCommerce.Sitefinity.UI.Mvc.Services;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Facet Filter MVC widget.
	/// </summary>
	public class FacetsFilterModel : IFacetsFilterModel
	{
		public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
		public IInsightsService Insights => UCommerceUIModule.Container.Resolve<IInsightsService>();

		// TODO: Check if we're manually sorting the products (as we do on the product listing page)
		public virtual IList<FacetViewModel> CreateViewModel()
		{
			Ucommerce.EntitiesV2.Category currentCategory = null;

			if (CatalogContext.CurrentCategory != null)
			{
				currentCategory = Ucommerce.EntitiesV2.Category.FirstOrDefault(c => c.Name == CatalogContext.CurrentCategory.Name);
				Insights.SendInteraction(currentCategory, "Filtered Product List", currentCategory.Name);
				return this.GetAllFacets(currentCategory);
			}

			var pageContext = SystemManager.CurrentHttpContext.GetProductsContext();
			var categoryIds = pageContext.GetValue<ProductsController>(p => p.CategoryIds);
			var productIds = pageContext.GetValue<ProductsController>(p => p.ProductIds);
			if (!string.IsNullOrEmpty(categoryIds) || !string.IsNullOrEmpty(productIds))
			{
				return this.MapFacetsByManualSelection(categoryIds, productIds);
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

		private IList<FacetViewModel> GetAllFacets(Ucommerce.EntitiesV2.Category category)
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
			var categories = Ucommerce.EntitiesV2.Category.Find(x => categoryIds.Contains(x.CategoryId));

			return this.MapToFacetsViewModel(
				CatalogLibrary.GetFacets(categories.Select(x => x.Guid).ToList(), facets.ToFacetDictionary()));
		}

		private IList<FacetViewModel> MapToFacetsViewModel(IList<Ucommerce.Search.Facets.Facet> facets)
		{
			var facetViewModel = facets.Select(x => new FacetViewModel()
			{
				DisplayName = x.DisplayName,
				Name = x.Name,
				FacetValues = x.FacetValues
					.Select(y => new FacetValue(y.Value, (int)y.Count))
					.ToList(),
			});

			var currentPriceGroup = Ucommerce.EntitiesV2.PriceGroup.FirstOrDefault(x => x.Guid == CatalogContext.CurrentPriceGroup.Guid);

			return (from f in facetViewModel
					where f.FacetValues.Any()
					let name = f.Name.ToLower()
					let isPriceFilter = name.Contains("price")
					where !isPriceFilter || (isPriceFilter && name.Contains("price") && name.EndsWith(currentPriceGroup.Name.ToLower()))
					select f
					).ToList();
		}

		private readonly IList<string> queryStringBlackList = new List<string>() { "product", "category", "catalog" };
	}
}
