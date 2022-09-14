using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using UCommerce.Sitefinity.UI.Constants;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using UCommerce.Sitefinity.UI.Search;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Content;
using Ucommerce.Extensions;
using Ucommerce.Search;
using Telerik.Sitefinity.Localization;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Search.Slugs;
using Ucommerce.Search.Extensions;
using Ucommerce.Search.Models;
using UCommerce.Sitefinity.UI.Mvc.Services;
using Product = Ucommerce.Search.Models.Product;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Product MVC widget.
	/// </summary>
	internal class ProductModel : IProductModel
	{
		public IIndex<Ucommerce.Search.Models.Product> ProductIndex =>
			ObjectFactory.Instance.Resolve<IIndex<Ucommerce.Search.Models.Product>>();
		private IIndexDefinition<Product> ProductIndexDefinition => ObjectFactory.Instance.Resolve<IIndexDefinition<Product>>();
		private ILocalizationContext LocalizationContext => ObjectFactory.Instance.Resolve<ILocalizationContext>();
		public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
		public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
		public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

		public ProductModel(int itemsPerPage, bool openInSamePage, bool isManualSelectionMode,
			bool enableCategoryFallback,
			Guid? detailsPageId = null, string productIds = null, string categoryIds = null,
			string fallbackCategoryIds = null)
		{
			this.itemsPerPage = itemsPerPage;
			this.openInSamePage = openInSamePage;
			this.isManualSelectionMode = isManualSelectionMode;
			this.enableCategoryFallback = enableCategoryFallback;
			this.detailsPageId = detailsPageId.HasValue ? detailsPageId.Value : Guid.Empty;
			this.productIds = productIds;
			this.categoryIds = categoryIds;
			this.fallbackCategoryIds = fallbackCategoryIds;
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

		public virtual ProductListViewModel CreateListViewModel()
		{
			var viewModel = new ProductListViewModel();
			viewModel.CurrentPage = this.GetRequestedPage();

			var currentCategory = CatalogContext.CurrentCategory;

			var searchTerm = System.Web.HttpContext.Current.Request.QueryString["search"];

			var queryResult = this.GetProductsQuery(currentCategory, searchTerm);
			this.ApplySorting(ref queryResult, ref viewModel);

			var itemsToSkip = viewModel.CurrentPage > 1 ? this.itemsPerPage * (viewModel.CurrentPage - 1) : 0;

			var products = queryResult
				.Skip(itemsToSkip)
				.Take(this.itemsPerPage)
				.ToList();

			viewModel.TotalCount = queryResult.Count();
			viewModel.Products = this.MapProducts(products, currentCategory, this.openInSamePage, this.detailsPageId);
			viewModel.TotalPagesCount = (viewModel.TotalCount + this.itemsPerPage - 1) / this.itemsPerPage;
			viewModel.ShowPager = viewModel.TotalPagesCount > 1;
			viewModel.PagingUrlTemplate = this.GetPagingUrlTemplate(currentCategory);
			viewModel.Routes.Add(RouteConstants.ADD_TO_BASKET_ROUTE_NAME, RouteConstants.ADD_TO_BASKET_ROUTE_VALUE);

			if (currentCategory != null)
			{
				var facets = HttpContext.Current.Request.QueryString.ToFacets();
				var actionName = facets?.Any() == true ? "Filter" : "View";
				InsightUcommerce.SendCategoryInteraction(currentCategory, $"{actionName} product list", currentCategory.Name);
			}

			return viewModel;
		}

		public virtual void ApplySorting(ref IQueryable<Ucommerce.Search.Models.Product> productsQuery,
			ref ProductListViewModel listVm)
		{
			var sortingOptions = new List<SortOption>();

			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "PriceAsc"), Key = "PriceAsc" });
			sortingOptions.Add(new SortOption()
			{ Title = Res.Get("UcommerceResources", "PriceDesc"), Key = "PriceDesc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "NameAsc"), Key = "NameAsc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "NameDesc"), Key = "NameDesc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "DateDesc"), Key = "DateDesc" });
			sortingOptions.Add(new SortOption()
			{ Title = Res.Get("UcommerceResources", "RatingDesc"), Key = "RatingDesc" });

			SortOption activeSortOption;
			var sortExpression = System.Web.HttpContext.Current.Request.QueryString["sortBy"];
			if (!string.IsNullOrWhiteSpace(sortExpression))
			{
				activeSortOption = sortingOptions.FirstOrDefault(s => s.Key == sortExpression);
				if (activeSortOption != null)
				{
					activeSortOption.IsActive = true;

					if (sortExpression == "PriceAsc")
					{
						productsQuery = productsQuery.OrderBy(p =>
							CatalogLibrary.CalculatePrices(new List<Guid>() { p.Guid }, null).Items.First()
								.ListPriceExclTax);
					}
					else if (sortExpression == "PriceDesc")
					{
						productsQuery = productsQuery.OrderByDescending(p =>
							CatalogLibrary.CalculatePrices(new List<Guid>() { p.Guid }, null).Items.First()
								.ListPriceExclTax);
					}
					else if (sortExpression == "NameAsc")
					{
						productsQuery = productsQuery.OrderBy(p => p.DisplayName);
					}
					else if (sortExpression == "NameDesc")
					{
						productsQuery = productsQuery.OrderByDescending(p => p.DisplayName);
					}
					//else if (sortExpression == "DateDesc")
					//{
					//	productsQuery = productsQuery.OrderByDescending(p => p.CreatedOn);
					//}
					else if (sortExpression == "RatingDesc")
					{
						productsQuery = productsQuery.OrderByDescending(p => p.Rating);
					}
				}
				else
				{
					// NOTE: Dates aren't available in v9 API yet
					//sortingOptions.Single(s => s.Key == "DateDesc").IsActive = true;
					//productsQuery = productsQuery.OrderByDescending(p => p.ModifiedOn);
					sortingOptions.Single(s => s.Key == "NameAsc").IsActive = true;
					productsQuery = productsQuery.OrderBy(p => p.DisplayName);
				}
			}
			else
			{
				// NOTE: Dates aren't available in v9 API yet
				//sortingOptions.Single(s => s.Key == "DateDesc").IsActive = true;
				//productsQuery = productsQuery.OrderByDescending(p => p.ModifiedOn);
				sortingOptions.Single(s => s.Key == "NameAsc").IsActive = true;
				productsQuery = productsQuery.OrderBy(p => p.DisplayName);
			}

			listVm.Sorting = sortingOptions;
		}

		public virtual ProductDetailViewModel CreateDetailsViewModel()
		{
ProductDetailViewModel productDetailViewModel = null;

			var currentProduct = CatalogContext.CurrentProduct;

			if (currentProduct != null)
			{
				var imageService = ObjectFactory.Instance.Resolve<IImageService>();
				var currentCategory = CatalogContext.CurrentCategory;

				var productPrice = CatalogLibrary.CalculatePrices(new List<Guid>() { currentProduct.Guid }).Items
					.FirstOrDefault();

				decimal price = 0;
				decimal discount = 0;

				if (productPrice != null)
				{
					price = productPrice.PriceExclTax;
					discount = productPrice.DiscountExclTax;
					var currentCatalog = CatalogContext.CurrentCatalog;
					if (currentCatalog != null && currentCatalog.ShowPricesIncludingTax)
					{
						price = productPrice.PriceInclTax;
						discount = productPrice.DiscountInclTax;
					}
				}

				var absoluteImageUrl = UrlPath.ResolveAbsoluteUrl(currentProduct.PrimaryImageUrl);

				productDetailViewModel = new ProductDetailViewModel()
				{
					DisplayName = currentProduct.DisplayName,
					Guid = currentProduct.Guid,
					PrimaryImageMediaUrl = absoluteImageUrl,
					LongDescription = currentProduct.LongDescription,
					ShortDescription = currentProduct.ShortDescription,
					ProductUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, CatalogContext.CurrentProduct),
					Price = new Money(price, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString(),
					Discount = discount > 0
						? new Money(discount, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString()
						: "",
					Sku = currentProduct.Sku,
					Rating = Convert.ToInt32(Math.Round(currentProduct.Rating.GetValueOrDefault())),
					VariantSku = currentProduct.VariantSku,
					IsVariant = currentProduct.ProductType == ProductType.Variant,
					IsProductFamily = currentProduct.ProductType == ProductType.ProductFamily,
					IsSellableProduct = currentProduct.ProductType == ProductType.ProductFamily || currentProduct.ProductType == ProductType.Variant
				};

				if (currentProduct.ParentProduct != null)
				{
                    var parentProduct = ProductIndex.Find<Product>()
                        .Where(product => product.Guid.Equals(currentProduct.ParentProduct)).First();
					productDetailViewModel.ParentProductUrl =
						UrlService.GetUrl(CatalogContext.CurrentCatalog, parentProduct);
					productDetailViewModel.ParentProductDisplayName = parentProduct.DisplayName;
				}

				if (currentCategory != null)
				{
					productDetailViewModel.CategoryDisplayName = currentCategory.DisplayName;
					productDetailViewModel.CategoryUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog,
						CatalogContext.CurrentCategories.Append(CatalogContext.CurrentCategory).Compact(),
						CatalogContext.CurrentProduct);
					productDetailViewModel.ProductUrl =
						UrlService.GetUrl(CatalogContext.CurrentCatalog, CatalogContext.CurrentProduct);
				}

				//Get Related Products
				if (currentProduct.RelatedProducts.Any())
				{
					var relatedIds = currentProduct.RelatedProducts;
					productDetailViewModel.RelatedProducts = this.ProductIndex.Find().Where(x => relatedIds.Contains(x.Guid)).ToList();
				}

				productDetailViewModel.ProductProperties = currentProduct.GetUserDefinedFields()
					.ToList();
				var variants = CatalogLibrary.GetVariants(currentProduct);

				if (variants.Count() != 0)
				{
					foreach (var userDefinedField in variants.First()
						         .GetUserDefinedFields())
					{
						var typeViewModel = new VariantTypeViewModel
						{
							DisplayName = ProductIndexDefinition.FieldDefinitions[userDefinedField.Key]
								.GetDisplayName(LocalizationContext.CurrentCulture.Name)
						};

						foreach (var variant in variants)
						{
							var variantViewModel = typeViewModel.Values
								.FirstOrDefault(v => v.Value.Equals(variant.DisplayName));

							if (variantViewModel == null)
							{
								var value = variant.GetUserDefinedFields()[userDefinedField.Key]
									.ToString();

								variantViewModel = new VariantViewModel
								{
									Value = value,
									TypeName = userDefinedField.Key
								};

								if (!string.IsNullOrEmpty(variant.PrimaryImageUrl))
								{
									variantViewModel.PrimaryImageMediaUrl = UrlPath.ResolveAbsoluteUrl(variant.PrimaryImageUrl);
								}

								typeViewModel.Values.Add(variantViewModel);
							}
						}
						var cleanedVariantViewModel = typeViewModel.Values.GroupBy(vvm => vvm.Value)
							.Select(grp => grp.FirstOrDefault())
							.ToList();
						typeViewModel.Values.Clear();
						typeViewModel.Values.AddRange(cleanedVariantViewModel);
						productDetailViewModel.VariantTypes.Add(typeViewModel);
					}
				}

				productDetailViewModel.Routes.Add(RouteConstants.ADD_TO_BASKET_ROUTE_NAME,
					RouteConstants.ADD_TO_BASKET_ROUTE_VALUE);
				InsightUcommerce.SendProductInteraction(currentProduct, "View product", $"{currentProduct.Name} ({currentProduct.Sku})");
			}

			return productDetailViewModel;
		}

		public virtual string GetProductUrl(Ucommerce.Search.Models.Category category,
			Ucommerce.Search.Models.Product product, bool openInSamePage, Guid detailPageId)
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

			string catUrl;
			var productCategory = category?.Guid ?? product?.Categories.FirstOrDefault();
			if (productCategory == null)
			{
				catUrl = CategoryModel.DefaultCategoryName;
			}
			else
			{
				catUrl = CategoryModel.GetCategoryPath(CatalogLibrary.GetCategory(productCategory));
			}

			var rawtUrl = string.Format("{0}/p/{1}", catUrl, product.Slug);
			var relativeUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", rawtUrl);

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

		private IQueryable<Ucommerce.Search.Models.Product> GetProductsQuery(Ucommerce.Search.Models.Category category,
			string searchTerm)
		{
			if (this.isManualSelectionMode)
			{
				var productGuids = this.productIds?.Split(',').Select(Guid.Parse).ToList() ?? new List<Guid>();
				var categoryGuids = this.categoryIds?.Split(',').Select(Guid.Parse).ToList() ?? new List<Guid>();

				return ApplyManualSelection(productGuids, categoryGuids);
			}

			if (string.IsNullOrWhiteSpace(searchTerm) && category == null && this.enableCategoryFallback == true)
			{
				var categoryGuids = this.fallbackCategoryIds?.Split(',').Select(Guid.Parse).ToList() ?? new List<Guid>();
				return ApplyManualSelection(new List<Guid>(), categoryGuids);
			}

			return ApplyAutoSelection(category, searchTerm);
		}

		private IQueryable<Ucommerce.Search.Models.Product> ApplyManualSelection(List<Guid> productIds,
			List<Guid> categoryIds)
		{
			var facets = HttpContext.Current.Request.QueryString.ToFacets();

			var products = ProductIndex.Find<Ucommerce.Search.Models.Product>()
				.Where(x => productIds.Contains(x.Guid) || categoryIds.Any(c => x.Categories.Contains(c)));

			if (facets != null && facets.Any())
			{
				return products.Where(facets.ToFacetDictionary()).ToFacets().AsQueryable();
			}

			return products.ToList().AsQueryable();
		}


		private IList<Ucommerce.EntitiesV2.Product> GetProductsFromSelectedCategoryIds(List<Guid> categoryIds)
		{
			List<Ucommerce.EntitiesV2.Product> result = new List<Ucommerce.EntitiesV2.Product>();
			var productsFromCategories = Ucommerce.EntitiesV2.CategoryProductRelation.All()
				.Where(x => categoryIds.Contains(x.Category.Guid))
				.ToList().GroupBy(x => x.Category);

			foreach (var productsFromCategory in productsFromCategories)
			{
				result.AddRange(productsFromCategory.Select(x => x.Product).ToList());
			}

			return result;
		}

		private ICollection<Ucommerce.Search.Models.Product> GetProductsFromSelectedProductIds(List<Guid> productIds)
		{
			return ProductIndex.Find<Ucommerce.Search.Models.Product>().Where(p => productIds.Contains(p.Guid)).ToList()
				.Results;
		}

		private IQueryable<Ucommerce.Search.Models.Product> ApplyAutoSelection(
			Ucommerce.Search.Models.Category currentCategory, string searchTerm)
		{
			var facets = HttpContext.Current.Request.QueryString.ToFacets();

			ISearch<Product> matchingProducts = ProductIndex.Find<Ucommerce.Search.Models.Product>()
				.Where(p => p.ProductType != ProductType.Variant);

			if (facets.Count > 0)
			{
				matchingProducts.Where(facets.ToFacetDictionary());
			}

			if (currentCategory != null)
			{
				matchingProducts = matchingProducts.Where(p => p.Categories.Contains(currentCategory.Guid));
			}

			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				matchingProducts = matchingProducts.Where(p =>
					 p.Sku.Contains(searchTerm)
					 || p.Name.Contains(searchTerm)
					 || p.DisplayName.Contains(searchTerm)
					 || p.ShortDescription.Contains(searchTerm)
					 || p.LongDescription.Contains(searchTerm)
				);
			}

			return matchingProducts.ToList().AsQueryable();
		}

		private IList<ProductDTO> MapProducts(IList<Ucommerce.Search.Models.Product> products,
			Ucommerce.Search.Models.Category category, bool openInSamePage, Guid detailPageId)
		{
			var result = new List<ProductDTO>();
			var currentCatalog = CatalogContext.CurrentCatalog;
			var productsPrices = CatalogLibrary.CalculatePrices(products.Select(x => x.Guid).ToList());

			foreach (var product in products)
			{
				var singleProductPrice = productsPrices.Items.FirstOrDefault(x => x.ProductGuid == product.Guid);

				decimal price = 0;
				decimal discount = 0;

				if (singleProductPrice != null)
				{
					price = singleProductPrice.PriceExclTax;
					discount = singleProductPrice.DiscountExclTax;
					if (currentCatalog.ShowPricesIncludingTax)
					{
						price = singleProductPrice.PriceInclTax;
						discount = singleProductPrice.DiscountInclTax;
					}
				}

				var productViewModel = new ProductDTO()
				{
					Sku = product.Sku,
					VariantSku = product.VariantSku,
					Price = new Money(price, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString(),
					Discount = discount > 0
						? new Money(discount, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString()
						: "",
					DisplayName = product.DisplayName,
					ThumbnailImageMediaUrl = product.ThumbnailImageUrl,
					ProductUrl = this.GetProductUrl(category, product, openInSamePage, detailPageId),
					IsSellableProduct = product.ProductType == ProductType.ProductFamily || product.ProductType == ProductType.Variant

				};

				result.Add(productViewModel);
			}

			return result;
		}

		private string GetPagingUrlTemplate(Ucommerce.Search.Models.Category category)
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
		private int itemsPerPage;
		private bool openInSamePage;
		private Guid detailsPageId;
		private bool isManualSelectionMode;
		private string productIds;
		private string categoryIds;
		private bool enableCategoryFallback;
		private string fallbackCategoryIds;
	}
}
