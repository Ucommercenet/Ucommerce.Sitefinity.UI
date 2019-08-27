using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce.Sitefinity.UI.Pages;
using Ucommerce.Sitefinity.UI.Search;
using UCommerce;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Runtime;
using UCommerce.Search;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    internal class ProductModel : IProductModel
    {
        public ProductModel(int itemsPerPage, bool openInSamePage, bool isManualSelectionMode, Guid? detailsPageId = null, string productIds = null, string categoryIds = null)
        {
            this.itemsPerPage = itemsPerPage;
            this.openInSamePage = openInSamePage;
            this.isManualSelectionMode = isManualSelectionMode;
            this.detailsPageId = detailsPageId.HasValue ? detailsPageId.Value : Guid.Empty;
            this.productIds = productIds;
            this.categoryIds = categoryIds;
        }

        public ProductListViewModel CreateListViewModel()
        {
            var viewModel = new ProductListViewModel();
            viewModel.CurrentPage = this.GetRequestedPage();

            var currentCategory = SiteContext.Current.CatalogContext.CurrentCategory;
            var queryResult = this.GetProductsQuery(currentCategory);

            var itemsToSkip = (viewModel.CurrentPage > 1) ? this.itemsPerPage * (viewModel.CurrentPage - 1) : 0;

            var products = queryResult
                            .Skip(itemsToSkip)
                            .Take(this.itemsPerPage)
                            .ToList();

            viewModel.TotalCount = queryResult.Count();
            viewModel.Products = this.MapProducts(products, currentCategory, this.openInSamePage, this.detailsPageId);
            viewModel.TotalPagesCount = (viewModel.TotalCount + this.itemsPerPage - 1) / this.itemsPerPage;
            viewModel.ShowPager = viewModel.TotalPagesCount > 1;
            viewModel.PagingUrlTemplate = this.GetPagingUrlTemplate(currentCategory);

            return viewModel;
        }

        public ProductDetailViewModel CreateDetailsViewModel()
        {
            ProductDetailViewModel productDetailViewModel = null;

            var currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            if (currentProduct != null)
            {
                var imageService = UCommerce.Infrastructure.ObjectFactory.Instance.Resolve<IImageService>();
                var currentCategory = SiteContext.Current.CatalogContext.CurrentCategory;
                string displayName = string.Empty;
                if (currentProduct.ParentProduct != null)
                {
                    displayName = $"{currentProduct.ParentProduct.DisplayName()} ";
                }

                displayName += currentProduct.DisplayName();

                var productPrice = CatalogLibrary.CalculatePrice(new List<Product>() { currentProduct }).Items.FirstOrDefault();

                decimal price = 0;
                decimal discount = 0;

                if (productPrice != null)
                {
                    price = productPrice.PriceExclTax;
                    discount = productPrice.DiscountExclTax;
                    var currentCatalog = SiteContext.Current.CatalogContext.CurrentCatalog;
                    if (currentCatalog != null && currentCatalog.ShowPricesIncludingVAT)
                    {
                        price = productPrice.PriceInclTax;
                        discount = productPrice.DiscountInclTax;
                    }
                }

                var imageUrl = imageService.GetImage(currentProduct.PrimaryImageMediaId).Url;
                var absoluteImageUrl = UrlPath.ResolveAbsoluteUrl(imageUrl);

                productDetailViewModel = new ProductDetailViewModel()
                {
                    DisplayName = displayName,
                    Guid = currentProduct.Guid,
                    PrimaryImageMediaUrl = absoluteImageUrl,
                    LongDescription = currentProduct.LongDescription(),
                    ShortDescription = currentProduct.ShortDescription(),
                    ProductUrl = CatalogLibrary.GetNiceUrlForProduct(currentProduct, currentCategory),
                    Price = new Money(price, SiteContext.Current.CatalogContext.CurrentPriceGroup.Currency).ToString(),
                    Discount = new Money(discount, SiteContext.Current.CatalogContext.CurrentPriceGroup.Currency).ToString(),
                    Sku = currentProduct.Sku,
                    Rating = Convert.ToInt32(Math.Round(currentProduct.Rating.GetValueOrDefault())),
                    VariantSku = currentProduct.VariantSku,
                    IsVariant = currentProduct.IsVariant,
                    IsProductFamily = currentProduct.ProductDefinition.IsProductFamily(),
                    AllowOrdering = currentProduct.AllowOrdering,
                };

                if (currentProduct.ParentProduct != null)
                {
                    productDetailViewModel.ParentProductUrl =
                        CatalogLibrary.GetNiceUrlForProduct(currentProduct.ParentProduct, currentCategory);
                    productDetailViewModel.ParentProductDisplayName = currentProduct.ParentProduct.DisplayName();
                }

                if (currentCategory != null)
                {
                    productDetailViewModel.CategoryDisplayName = currentCategory.DisplayName();
                    productDetailViewModel.CategoryUrl = CatalogLibrary.GetNiceUrlForCategory(currentCategory);
                    productDetailViewModel.ProductUrl = CatalogLibrary.GetNiceUrlForProduct(currentProduct, currentCategory);
                }

                foreach (var variant in currentProduct.Variants)
                {
                    var variantViewModel = new ProductDetailViewModel
                    {
                        Sku = variant.Sku,
                        Guid = variant.Guid,
                        VariantSku = variant.VariantSku,
                        DisplayName = variant.DisplayName(),
                        IsVariant = true,
                        AllowOrdering = variant.AllowOrdering,
                    };

                    if (!string.IsNullOrEmpty(variant.PrimaryImageMediaId))
                    {
                        var variantImageUrl = imageService.GetImage(variant.PrimaryImageMediaId).Url;
                        variantViewModel.PrimaryImageMediaUrl = UrlPath.ResolveAbsoluteUrl(variantImageUrl);
                    }

                    productDetailViewModel.Variants.Add(variantViewModel);
                }
            }

            return productDetailViewModel;
        }

        public string GetProductUrl(Category category, Product product, bool openInSamePage, Guid detailPageId)
        {
            var baseUrl = string.Empty;
            if (openInSamePage)
            {
                baseUrl = UrlResolver.GetCurrentPageNodeUrl();
            }
            else
            {
                baseUrl = UrlResolver.GetPageNodeUrl(detailPageId);
            }

            string categoryName;
            var productCategory = category ?? product.GetCategories().FirstOrDefault();
            if (productCategory == null)
            {
                categoryName = "general";
            }
            else
            {
                categoryName = productCategory.Name;
            }

            var rawtUrl = string.Format("{0}/{1}", categoryName, product.ProductId);
            var sanitizedUrl = ControlUtilities.SanitizeUrl(rawtUrl);
            string relativeUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", sanitizedUrl);

            string url;

            if (SystemManager.CurrentHttpContext.Request.Url != null)
            {
                url = UrlPath.ResolveUrl(relativeUrl, true);
            }
            else
            {
                url = UrlResolver.GetAbsoluteUrl(relativeUrl);
            }

            return url;
        }

        protected virtual IQueryable<Product> FilterByFacets(Category category, List<Product> productsInCategory)
        {
            var facetsResolver = new FacetResolver(this.queryStringBlackList);
            var facetsForQuerying = facetsResolver.GetFacetsFromQueryString();
            var filterProducts = SearchLibrary.GetProductsFor(category, facetsForQuerying);

            if (!filterProducts.Any())
            {
                return productsInCategory.AsQueryable();
            }

            var listOfProducts = new List<Product>();

            foreach (var product in filterProducts)
            {
                var filterProduct =
                    productsInCategory.FirstOrDefault(x => x.Sku == product.Sku && x.VariantSku == product.VariantSku);
                if (filterProduct != null)
                {
                    listOfProducts.Add(filterProduct);
                }
            }

            return listOfProducts.AsQueryable();
        }

        private IQueryable<Product> GetProductsQuery(Category category)
        {
            if (this.isManualSelectionMode)
            {
                return this.ApplyManualSelection();
            }
            else
            {
                return this.ApplyAutoSelection(category);
            }
        }

        private IQueryable<Product> ApplyManualSelection()
        {
            var productsQuery = SearchLibrary.FacetedQuery();
            var productIds = this.productIds?.Split(',').Select(x => Convert.ToInt32(x)).ToList() ?? new List<int>();
            var categoryIds = this.categoryIds?.Split(',').Select(x => Convert.ToInt32(x)).ToList() ?? new List<int>();

            var products = new List<Product>();
            products.AddRange(this.GetProductsFromSelectedCategoryIds(categoryIds));
            products.AddRange(this.GetProductsFromSelectedProductIds(productIds));

            var facetsResolver = new FacetResolver(this.queryStringBlackList);

            List<UCommerce.Documents.Product> productsFromFacets = SearchLibrary.FacetedQuery()
                .Where(x => x.CategoryIds.In(categoryIds) || x.Id.In(productIds))
                .WithFacets(facetsResolver.GetFacetsFromQueryString())
                .ToList();

            if (!productsFromFacets.Any())
                return products.AsQueryable();

            return products.Where(x => productsFromFacets.Any(y => x.Sku == y.Sku)).AsQueryable();
        }

        private IList<Product> GetProductsFromSelectedCategoryIds(List<int> categoryIds)
        {
            List<Product> result = new List<Product>();
            var productsFromCategories = CategoryProductRelation.All()
                    .Where(x => categoryIds.Contains(x.Category.CategoryId))
                    .ToList().GroupBy(x => x.Category);

            foreach (var productsFromCategory in productsFromCategories)
            {
                result.AddRange(productsFromCategory.Select(x => x.Product).ToList());
            }

            return result;
        }

        private IList<Product> GetProductsFromSelectedProductIds(List<int> productIds)
        {
            return Product.All().Where(x => productIds.Contains(x.ProductId)).ToList();
        }

        private IQueryable<Product> ApplyAutoSelection(Category currentCategory)
        {
            if (currentCategory == null)
            {
                var catalog = CatalogLibrary.GetAllCatalogs().FirstOrDefault();
                if (catalog != null)
                {
                    currentCategory = CatalogLibrary.GetRootCategories(catalog.Id).FirstOrDefault();
                    if (currentCategory == null)
                    {
                        throw new InvalidOperationException(NO_CATEGORIES_ERROR_MESSAGE);
                    }
                }
                else
                {
                    throw new InvalidOperationException(NO_CATALOG_ERROR_MESSAGE);
                }
            }

            var productsInCategory = CatalogLibrary.GetProducts(currentCategory).ToList();
            return this.FilterByFacets(currentCategory, productsInCategory);
        }

        private IList<ProductViewModel> MapProducts(IList<Product> products, Category category, bool openInSamePage, Guid detailPageId)
        {
            var result = new List<ProductViewModel>();
            var imageService = UCommerce.Infrastructure.ObjectFactory.Instance.Resolve<IImageService>();
            var currentCatalog = SiteContext.Current.CatalogContext.CurrentCatalog;
            var productsPrices = CatalogLibrary.CalculatePrice(products.Select(x => x.Guid).ToList());

            foreach (var product in products)
            {
                var singleProductPrice = productsPrices.Items.Where(x => x.ProductGuid == product.Guid).FirstOrDefault();

                decimal price = 0;
                decimal discount = 0;

                if (singleProductPrice != null)
                {
                    price = singleProductPrice.PriceExclTax;
                    discount = singleProductPrice.DiscountExclTax;
                    if (currentCatalog.ShowPricesIncludingVAT)
                    {
                        price = singleProductPrice.PriceInclTax;
                        discount = singleProductPrice.DiscountInclTax;
                    }

                }

                var productViewModel = new ProductViewModel()
                {
                    Sku = product.Sku,
                    VariantSku = product.VariantSku,
                    Price = new Money(price, SiteContext.Current.CatalogContext.CurrentPriceGroup.Currency).ToString(),
                    Discount = new Money(discount, SiteContext.Current.CatalogContext.CurrentPriceGroup.Currency).ToString(),
                    DisplayName = product.DisplayName(),
                    ThumbnailImageMediaUrl = imageService.GetImage(product.ThumbnailImageMediaId).Url,
                    ProductUrl = this.GetProductUrl(category, product, openInSamePage, detailPageId),
                    IsSellableProduct = (product.ProductDefinition.IsProductFamily() && product.IsVariant) || (!product.ProductDefinition.IsProductFamily() && !product.IsVariant),
                };

                result.Add(productViewModel);
            }

            return result;
        }

        private string GetPagingUrlTemplate(Category category)
        {
            string url;

            if (SystemManager.CurrentHttpContext.Request.Url != null)
            {
                var queryParams = HttpUtility.ParseQueryString(SystemManager.CurrentHttpContext.Request.Url.Query);
                queryParams[PAGER_QUERY_STRING_KEY] = "{0}";

                var queryParamsRaw = HttpUtility.UrlDecode(queryParams.ToQueryString(true));
                url = string.Concat(SystemManager.CurrentHttpContext.Request.Url.LocalPath, queryParamsRaw);
            }
            else
            {
                url = UrlResolver.GetAbsoluteUrl(SiteMapBase.GetActualCurrentNode().Url + "?{0}");
            }

            return url;
        }

        private int GetRequestedPage()
        {
            var page = 1;
            var currentUrl = SystemManager.CurrentHttpContext.Request.Url;

            if (currentUrl != null)
            {
                var qs = HttpUtility.ParseQueryString(currentUrl.Query);
                if (qs[PAGER_QUERY_STRING_KEY] != null)
                    page = int.Parse(qs[PAGER_QUERY_STRING_KEY]);
            }

            return page;
        }

        public const string PAGER_QUERY_STRING_KEY = "page";
        private const string NO_CATALOG_ERROR_MESSAGE = "There is no product catalog configured.";
        private const string NO_CATEGORIES_ERROR_MESSAGE = "There are no product categories configured.";
        private IList<string> queryStringBlackList = new List<string>() { "product", "category", "catalog", "page" };
        private int itemsPerPage;
        private bool openInSamePage;
        private Guid detailsPageId;
        private bool isManualSelectionMode;
        private string productIds;
        private string categoryIds;
    }
}