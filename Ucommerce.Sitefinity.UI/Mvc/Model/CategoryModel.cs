using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Ucommerce.Sitefinity.UI.Constants;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce.Sitefinity.UI.Pages;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Runtime;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    internal class CategoryModel : ICategoryModel
    {
        public CategoryModel(bool hideMiniBasket, bool allowChangingCurrency, Guid? imageId = null, Guid? categoryPageId = null, Guid? searchPageId = null)
        {
            this.hideMiniBasket = hideMiniBasket;
            this.allowChangingCurrency = allowChangingCurrency;
            this.imageId = imageId ?? Guid.Empty;
            this.categoryPageId = categoryPageId ?? Guid.Empty;
            this.searchPageId = searchPageId ?? Guid.Empty;
        }

        public CategoryNavigationViewModel CreateViewModel()
        {
            var categoryNavigationViewModel = new CategoryNavigationViewModel();
            var rootCategories = CatalogLibrary.GetRootCategories().Where(x => x.DisplayOnSite).ToList();
            var currentPriceGroup = SiteContext.Current.CatalogContext.CurrentPriceGroup;
            categoryNavigationViewModel.Categories = this.MapCategories(rootCategories, SiteContext.Current.CatalogContext.CurrentCategory);
            categoryNavigationViewModel.Currencies = this.MapCurrencies(SiteContext.Current.CatalogContext.CurrentCatalog.AllowedPriceGroups, currentPriceGroup);
            categoryNavigationViewModel.CurrentCurrency = new CategoryNavigationCurrencyViewModel()
            {
                DisplayName = currentPriceGroup.Name,
                PriceGroupId = currentPriceGroup.PriceGroupId,
            };

            this.MapConfigurationFields(categoryNavigationViewModel);

            categoryNavigationViewModel.Routes.Add(RouteConstants.SEARCH_ROUTE_NAME, RouteConstants.SEARCH_ROUTE_VALUE);
            categoryNavigationViewModel.Routes.Add(RouteConstants.SEARCH_SUGGESTIONS_ROUTE_NAME, RouteConstants.SEARCH_SUGGESTIONS_ROUTE_VALUE);
            categoryNavigationViewModel.Routes.Add(RouteConstants.PRICE_GROUP_ROUTE_NAME, RouteConstants.PRICE_GROUP_ROUTE_VALUE);
            categoryNavigationViewModel.Routes.Add(RouteConstants.GET_BASKET_ROUTE_NAME, RouteConstants.GET_BASKET_ROUTE_VALUE);

            return categoryNavigationViewModel;
        }

        protected virtual void MapConfigurationFields(CategoryNavigationViewModel categoryNavigationViewModel)
        {
            categoryNavigationViewModel.HideMiniBasket = this.hideMiniBasket;
            categoryNavigationViewModel.AllowChangingCurrency = this.allowChangingCurrency;

            if (this.imageId != Guid.Empty)
            {
                try
                {
                    var manager = LibrariesManager.GetManager();
                    var image = manager.GetImage(this.imageId);
                    categoryNavigationViewModel.ImageUrl = MediaContentExtensions.ResolveMediaUrl(image, false);
                }
                catch (Exception ex)
                {
                    Log.Write($"Categories Model: Image cannot be retrieved. Cannot resolve image with Id: {this.imageId} due to the following exception: {Environment.NewLine} {ex}", ConfigurationPolicy.ErrorLog);
                }
            }

            if (this.searchPageId != Guid.Empty)
            {
                var nextSearchUrl = UrlResolver.GetPageNodeUrl(this.searchPageId);

                if (nextSearchUrl != null)
                    categoryNavigationViewModel.SearchPageUrl = nextSearchUrl;
            }
        }

        protected virtual IList<CategoryNavigationCategoryViewModel> MapCategories(ICollection<Category> rootCategories, Category currentCategory)
        {
            var result = new List<CategoryNavigationCategoryViewModel>();

            foreach (var category in rootCategories)
            {
                result.Add(new CategoryNavigationCategoryViewModel()
                {
                    DisplayName = category.DisplayName(),
                    Url = this.GetCategoryUrl(category.Name),
                    Categories = this.MapCategories(category.Categories.Where(x => x.DisplayOnSite).ToList(), currentCategory),
                    IsActive = currentCategory != null && currentCategory.Guid == category.Guid,
                });
            }

            return result;
        }

        private IList<CategoryNavigationCurrencyViewModel> MapCurrencies(ICollection<PriceGroup> currentCatalogAllowedPriceGroups, PriceGroup currentPriceGroup)
        {
            var categoryNavigationCurrencyViewModels = new List<CategoryNavigationCurrencyViewModel>();

            foreach (var currentCatalogAllowedPriceGroup in currentCatalogAllowedPriceGroups)
            {
                if (currentPriceGroup == currentCatalogAllowedPriceGroup) continue;
                var categoryNavigationCurrencyViewModel = new CategoryNavigationCurrencyViewModel();

                categoryNavigationCurrencyViewModel.DisplayName = currentCatalogAllowedPriceGroup.Name;
                categoryNavigationCurrencyViewModel.PriceGroupId = currentCatalogAllowedPriceGroup.PriceGroupId;

                categoryNavigationCurrencyViewModels.Add(categoryNavigationCurrencyViewModel);
            }

            return categoryNavigationCurrencyViewModels;
        }

        private string GetCategoryUrl(string categoryName)
        {
            var baseUrl = string.Empty;
            if (this.categoryPageId == Guid.Empty)
            {
                baseUrl = UrlResolver.GetCurrentPageNodeUrl();
            }
            else
            {
                baseUrl = UrlResolver.GetPageNodeUrl(this.categoryPageId);
            }

            var relativeCategoryUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", categoryName);
            string url;

            if (SystemManager.CurrentHttpContext.Request.Url != null)
            {
                url = UrlPath.ResolveUrl(relativeCategoryUrl, true);
            }
            else
            {
                url = UrlResolver.GetAbsoluteUrl(SiteMapBase.GetActualCurrentNode().Url);
            }

            return url;
        }

        private Guid imageId;
        private bool hideMiniBasket;
        private bool allowChangingCurrency;
        private Guid categoryPageId;
        private Guid searchPageId;
    }
}
