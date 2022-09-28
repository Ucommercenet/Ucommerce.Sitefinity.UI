using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Ucommerce.Api;
using Ucommerce.Search.Models;
using UCommerce.Sitefinity.UI.Constants;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using ObjectFactory = Ucommerce.Infrastructure.ObjectFactory;
using PriceGroup = Ucommerce.EntitiesV2.PriceGroup;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Category MVC widget.
    /// </summary>
    internal class CategoryModel : ICategoryModel
    {
        internal static string DefaultCategoryName = "Category";
        private readonly bool allowChangingCurrency;
        private readonly Guid categoryPageId;
        private readonly bool hideMiniBasket;
        private readonly Guid imageId;
        private readonly Guid productDetailsPageId;
        private readonly Guid searchPageId;
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();

        public CategoryModel(bool hideMiniBasket,
            bool allowChangingCurrency,
            Guid? imageId = null,
            Guid? categoryPageId = null,
            Guid? searchPageId = null,
            Guid? productDetailsPageId = null)
        {
            this.hideMiniBasket = hideMiniBasket;
            this.allowChangingCurrency = allowChangingCurrency;
            this.imageId = imageId ?? Guid.Empty;
            this.categoryPageId = categoryPageId ?? Guid.Empty;
            this.searchPageId = searchPageId ?? Guid.Empty;
            this.productDetailsPageId = productDetailsPageId ?? Guid.Empty;
        }

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

        public virtual CategoryNavigationViewModel CreateViewModel()
        {
            var categoryNavigationViewModel = new CategoryNavigationViewModel();
            // HACK: This is a temporary work around while we dig into the best way to do this with the new API
            var rootCategories = CatalogLibrary.GetRootCategories()
                .ToList();
            var currentCategory = CatalogContext.CurrentCategory;
            categoryNavigationViewModel.Categories = MapCategories(rootCategories, currentCategory);

            var priceGroups = PriceGroup.Find(x => CatalogContext.CurrentCatalog.PriceGroups.Contains(x.Guid));
            var currentPriceGroup = PriceGroup.FirstOrDefault(x => x.Guid == CatalogContext.CurrentPriceGroup.Guid);
            categoryNavigationViewModel.Currencies = MapCurrencies(priceGroups, currentPriceGroup);
            categoryNavigationViewModel.Localizations = GetCurrentCulture();
            categoryNavigationViewModel.CurrentCurrency = new CategoryNavigationCurrencyViewModel
            {
                DisplayName = currentPriceGroup.Name,
                PriceGroupId = currentPriceGroup.PriceGroupId,
            };
            GetCurrentCulture();

            categoryNavigationViewModel.ProductDetailsPageId = productDetailsPageId;

            MapConfigurationFields(categoryNavigationViewModel);

            categoryNavigationViewModel.Routes.Add(RouteConstants.SEARCH_ROUTE_NAME, RouteConstants.SEARCH_ROUTE_VALUE);
            categoryNavigationViewModel.Routes.Add(RouteConstants.SEARCH_SUGGESTIONS_ROUTE_NAME,
                RouteConstants.SEARCH_SUGGESTIONS_ROUTE_VALUE);
            categoryNavigationViewModel.Routes.Add(RouteConstants.PRICE_GROUP_ROUTE_NAME,
                RouteConstants.PRICE_GROUP_ROUTE_VALUE);
            categoryNavigationViewModel.Routes.Add(RouteConstants.GET_BASKET_ROUTE_NAME,
                RouteConstants.GET_BASKET_ROUTE_VALUE);

            if (categoryPageId == Guid.Empty)
            {
                categoryNavigationViewModel.BaseUrl = UrlResolver.GetCurrentPageNodeUrl();
            }
            else
            {
                categoryNavigationViewModel.BaseUrl = UrlResolver.GetAbsoluteUrl(UrlResolver.GetPageNodeUrl(categoryPageId));
            }

            return categoryNavigationViewModel;
        }

        protected virtual string GetCategoryPath(string path, Category category)
        {
            if (path == "")
            {
                return category.Name;
            }

            return $"{path}/{category.Name}";
        }

        protected virtual IList<CategoryNavigationCategoryViewModel> MapCategories(IList<Category> rootCategories,
            Category currentCategory,
            string parentCategoryRelativePath = "")
        {
            var result = new List<CategoryNavigationCategoryViewModel>();

            var subCategories = CatalogLibrary.GetCategories(rootCategories.SelectMany(x => x.Categories)
                    .ToList())
                .ToList();
            foreach (var category in rootCategories)
            {
                var relativeCategoryPath = GetCategoryPath(parentCategoryRelativePath, category);
                result.Add(new CategoryNavigationCategoryViewModel
                {
                    CategoryId = category.Guid,
                    DisplayName = category.DisplayName,
                    Url = GetAbsoluteCategoryUrl(relativeCategoryPath),
                    Categories = MapCategories(subCategories.Where(x => category.Categories.Any(y => y.Equals(x.Guid)))
                            .ToList(),
                        currentCategory,
                        relativeCategoryPath),
                    IsActive = currentCategory != null && currentCategory.Guid == category.Guid,
                });
            }

            return result;
        }

        protected virtual void MapConfigurationFields(CategoryNavigationViewModel categoryNavigationViewModel)
        {
            categoryNavigationViewModel.HideMiniBasket = hideMiniBasket;
            categoryNavigationViewModel.AllowChangingCurrency = allowChangingCurrency;

            if (imageId != Guid.Empty)
            {
                try
                {
                    var manager = LibrariesManager.GetManager();
                    var image = manager.GetImage(imageId);
                    categoryNavigationViewModel.ImageUrl = image.ResolveMediaUrl();
                }
                catch (Exception ex)
                {
                    Log.Write(
                        $"Categories Model: Image cannot be retrieved. Cannot resolve image with Id: {imageId} due to the following exception: {Environment.NewLine} {ex}",
                        ConfigurationPolicy.ErrorLog);
                }
            }

            if (searchPageId != Guid.Empty)
            {
                var nextSearchUrl = UrlResolver.GetPageNodeUrl(searchPageId);

                if (nextSearchUrl != null)
                {
                    categoryNavigationViewModel.SearchPageUrl = nextSearchUrl;
                }
            }
        }

        protected virtual IList<CategoryNavigationCurrencyViewModel> MapCurrencies(ICollection<PriceGroup> currentCatalogAllowedPriceGroups,
            PriceGroup currentPriceGroup)
        {
            var categoryNavigationCurrencyViewModels = new List<CategoryNavigationCurrencyViewModel>();

            foreach (var currentCatalogAllowedPriceGroup in currentCatalogAllowedPriceGroups)
            {
                if (currentPriceGroup == currentCatalogAllowedPriceGroup)
                {
                    continue;
                }

                var categoryNavigationCurrencyViewModel = new CategoryNavigationCurrencyViewModel();

                categoryNavigationCurrencyViewModel.DisplayName = currentCatalogAllowedPriceGroup.Name;
                categoryNavigationCurrencyViewModel.PriceGroupId = currentCatalogAllowedPriceGroup.PriceGroupId;

                categoryNavigationCurrencyViewModels.Add(categoryNavigationCurrencyViewModel);
            }

            return categoryNavigationCurrencyViewModels;
        }

        private string GetAbsoluteCategoryUrl(string relativeCategoryPath)
        {
            string baseUrl;
            if (categoryPageId == Guid.Empty)
            {
                baseUrl = UrlResolver.GetCurrentPageNodeUrl();
            }
            else
            {
                baseUrl = UrlResolver.GetPageNodeUrl(categoryPageId);
            }

            var relativeUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", relativeCategoryPath);

            if (SystemManager.CurrentHttpContext.Request.Url != null)
            {
                return UrlPath.ResolveUrl(relativeUrl, true);
            }

            return UrlResolver.GetAbsoluteUrl(SiteMapBase.GetActualCurrentNode()
                .Url);
        }

        private string GetCurrentCulture()
        {
            var actualSiteMapNode = SiteMapBase.GetActualCurrentNode();
            var localization = new Dictionary<string, string>();
            var siteMapProvider = SiteMapBase.GetCurrentProvider();
            var currentPageSiteNode = siteMapProvider.CurrentNode as PageSiteNode;

            if (actualSiteMapNode != null)
            {
                if (currentPageSiteNode != null)
                {
                    foreach (var availableLanguage in currentPageSiteNode.AvailableLanguages)
                    {
                        localization.Add(availableLanguage.Name, currentPageSiteNode.GetUrl(availableLanguage));
                    }
                }
            }

            return JsonConvert.SerializeObject(localization, Formatting.Indented);
        }

        internal static string GetCategoryPath(Category category)
        {
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();

            var catNames = new List<string>();
            var cat = category;

            while (cat != null)
            {
                catNames.Add(cat.Name);
                if (cat.ParentCategory.HasValue)
                {
                    cat = catalogLibrary.GetCategories(new List<Guid> { cat.ParentCategory.Value })
                        .FirstOrDefault();
                }
                else
                {
                    cat = null;
                }
            }

            catNames.Reverse();
            var catUrl = string.Join("/", catNames.ToArray());

            return catUrl;
        }
    }
}
