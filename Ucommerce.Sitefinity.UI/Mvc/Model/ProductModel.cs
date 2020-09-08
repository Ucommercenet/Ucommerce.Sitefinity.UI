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
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Search;
using Telerik.Sitefinity.Localization;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Slugs;
using Ucommerce.Search.Extensions;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Product MVC widget.
	/// </summary>
	internal class ProductModel : IProductModel
	{
		public IIndex<Ucommerce.Search.Models.Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Ucommerce.Search.Models.Product>>();
		public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
		public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

		public ProductModel(int itemsPerPage, bool openInSamePage, bool isManualSelectionMode, bool enableCategoryFallback,
			Guid? detailsPageId = null, string productIds = null, string categoryIds = null, string fallbackCategoryIds = null)
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

			var currentCategory = CatalogContext.CurrentCategory ?? CatalogLibrary.GetRootCategories().FirstOrDefault();

			var searchTerm = System.Web.HttpContext.Current.Request.QueryString["search"];

			var queryResult = this.GetProductsQuery(currentCategory, searchTerm);
			this.ApplySorting(ref queryResult, ref viewModel);

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
			viewModel.Routes.Add(RouteConstants.ADD_TO_BASKET_ROUTE_NAME, RouteConstants.ADD_TO_BASKET_ROUTE_VALUE);

			return viewModel;
		}

		public virtual void ApplySorting(ref IQueryable<Ucommerce.Search.Models.Product> productsQuery, ref ProductListViewModel listVm)
		{
			var sortingOptions = new List<SortOption>();

			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "PriceAsc"), Key = "PriceAsc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "PriceDesc"), Key = "PriceDesc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "NameAsc"), Key = "NameAsc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "NameDesc"), Key = "NameDesc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "DateDesc"), Key = "DateDesc" });
			sortingOptions.Add(new SortOption() { Title = Res.Get("UcommerceResources", "RatingDesc"), Key = "RatingDesc" });

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
						productsQuery = productsQuery.OrderBy(p => CatalogLibrary.CalculatePrices(new List<Guid>() { p.Guid }, null).Items.First().ListPriceExclTax);
					}
					else if (sortExpression == "PriceDesc")
					{
						productsQuery = productsQuery.OrderByDescending(p => CatalogLibrary.CalculatePrices(new List<Guid>() { p.Guid }, null).Items.First().ListPriceExclTax);
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

			var currentProduct = Ucommerce.EntitiesV2.Product.FirstOrDefault(x => x.Guid == CatalogContext.CurrentProduct.Guid);

			if (currentProduct != null)
			{
				var imageService = ObjectFactory.Instance.Resolve<IImageService>();
				var currentCategory = CatalogContext.CurrentCategory;
				string displayName = string.Empty;
				if (currentProduct.ParentProduct != null)
				{
					displayName = $"{currentProduct.ParentProduct.DisplayName()} ";
				}

				displayName += currentProduct.DisplayName();

				var productPrice = CatalogLibrary.CalculatePrices(new List<Guid>() { currentProduct.Guid }).Items.FirstOrDefault();

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

				var imageUrl = imageService.GetImage(currentProduct.PrimaryImageMediaId).Url;
				var absoluteImageUrl = UrlPath.ResolveAbsoluteUrl(imageUrl);

				productDetailViewModel = new ProductDetailViewModel()
				{
					DisplayName = displayName,
					Guid = currentProduct.Guid,
					PrimaryImageMediaUrl = absoluteImageUrl,
					LongDescription = currentProduct.LongDescription(),
					ShortDescription = currentProduct.ShortDescription(),
					ProductUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, CatalogContext.CurrentProduct),
					Price = new Money(price, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString(),
					Discount = discount > 0 ? new Money(discount, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString() : "",
					Sku = currentProduct.Sku,
					Rating = Convert.ToInt32(Math.Round(currentProduct.Rating.GetValueOrDefault())),
					VariantSku = currentProduct.VariantSku,
					IsVariant = currentProduct.IsVariant,
					IsProductFamily = currentProduct.ProductDefinition.IsProductFamily(),
					AllowOrdering = currentProduct.AllowOrdering,
				};

				if (currentProduct.ParentProduct != null)
				{
					var parentProduct = CatalogLibrary.GetProduct(currentProduct.ParentProduct.Sku);
					productDetailViewModel.ParentProductUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, parentProduct);
					productDetailViewModel.ParentProductDisplayName = currentProduct.ParentProduct.DisplayName();
				}

				if (currentCategory != null)
				{
					productDetailViewModel.CategoryDisplayName = currentCategory.DisplayName;
					productDetailViewModel.CategoryUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog,
						CatalogContext.CurrentCategories.Append(CatalogContext.CurrentCategory).Compact(), CatalogContext.CurrentProduct);
					productDetailViewModel.ProductUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, CatalogContext.CurrentProduct);
				}

				var invariantFields = currentProduct.ProductProperties;

				var localizationContext = ObjectFactory.Instance.Resolve<ILocalizationContext>();

				var fieldsForCurrentLanguage = currentProduct.GetProperties(localizationContext.CurrentCultureCode).ToList();

				productDetailViewModel.ProductProperties = invariantFields.Concat(fieldsForCurrentLanguage).ToList();


				var uniqueVariants = from v in currentProduct.Variants.SelectMany(p => p.ProductProperties)
									 where v.ProductDefinitionField.DisplayOnSite
									 group v by v.ProductDefinitionField into g
									 select g;

				foreach (var vt in uniqueVariants)
				{
					var typeViewModel = productDetailViewModel.VariantTypes
															.Where(z => z.Id == vt.Key.ProductDefinitionFieldId)
															.FirstOrDefault();

					if (typeViewModel == null)
					{
						typeViewModel = new VariantTypeViewModel
						{
							Id = vt.Key.ProductDefinitionFieldId,
							Name = vt.Key.Name,
							DisplayName = vt.Key.GetDisplayName()
						};

						productDetailViewModel.VariantTypes.Add(typeViewModel);
					}

					var variants = vt.ToList();

					foreach (var variant in variants)
					{
						var variantViewModel = typeViewModel.Values
														  .Where(v => v.Value == variant.Value)
														  .FirstOrDefault();

						if (variantViewModel == null)
						{
							variantViewModel = new VariantViewModel
							{
								Value = variant.Value,
								TypeName = typeViewModel.Name
							};

							if (!string.IsNullOrEmpty(variant.Product.PrimaryImageMediaId))
							{
								var variantImageUrl = imageService.GetImage(variant.Product.PrimaryImageMediaId).Url;
								variantViewModel.PrimaryImageMediaUrl = UrlPath.ResolveAbsoluteUrl(variantImageUrl);
							}

							typeViewModel.Values.Add(variantViewModel);
						}
					}
				}

				productDetailViewModel.Routes.Add(RouteConstants.ADD_TO_BASKET_ROUTE_NAME, RouteConstants.ADD_TO_BASKET_ROUTE_VALUE);
			}

			return productDetailViewModel;
		}

		public virtual string GetProductUrl(Ucommerce.Search.Models.Category category, Ucommerce.Search.Models.Product product, bool openInSamePage, Guid detailPageId)
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
			string relativeUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", rawtUrl);

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

		private IQueryable<Ucommerce.Search.Models.Product> GetProductsQuery(Ucommerce.Search.Models.Category category, string searchTerm)
		{
			if (this.isManualSelectionMode)
			{
				// NOTE: The int values will go away soon but the picker still saves as ints at the moment so we need to convert them
				var productIds = this.productIds?.Split(',').Select(Int32.Parse).ToList() ?? new List<int>();
				var categoryIds = this.categoryIds?.Split(',').Select(Int32.Parse).ToList() ?? new List<int>();

				var productGuids = Ucommerce.EntitiesV2.Product.Find(p => productIds.Contains(p.ProductId)).Select(p => p.Guid).ToList();
				var categoryGuids = Ucommerce.EntitiesV2.Category.Find(c => categoryIds.Contains(c.CategoryId)).Select(c => c.Guid).ToList();

				return ApplyManualSelection(productGuids, categoryGuids);
			}

			if (string.IsNullOrWhiteSpace(searchTerm) && category == null && this.enableCategoryFallback == true)
			{
				var categoryIds = this.fallbackCategoryIds?.Split(',').Select(Int32.Parse).ToList() ?? new List<int>();
				var categoryGuids = Ucommerce.EntitiesV2.Category.Find(c => categoryIds.Contains(c.CategoryId)).Select(c => c.Guid).ToList();
				return ApplyManualSelection(new List<Guid>(), categoryGuids);
			}

			return ApplyAutoSelection(category, searchTerm);
		}

		private IQueryable<Ucommerce.Search.Models.Product> ApplyManualSelection(List<Guid> productIds, List<Guid> categoryIds)
		{
			var facets = HttpContext.Current.Request.QueryString.ToFacets();

			if (categoryIds.Any())
			{
				// HACK: We need to find a way around this as it uses EntitiesV2
				productIds.AddRange(GetProductsFromSelectedCategoryIds(categoryIds).Select(p => p.Guid).ToList());
			}

			var products = ProductIndex.Find<Ucommerce.Search.Models.Product>()
									   .Where(x => productIds.Contains(x.Guid));

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
			return ProductIndex.Find<Ucommerce.Search.Models.Product>().Where(p => productIds.Contains(p.Guid)).ToList().Results;
		}

		private IQueryable<Ucommerce.Search.Models.Product> ApplyAutoSelection(Ucommerce.Search.Models.Category currentCategory, string searchTerm)
		{
			var facets = HttpContext.Current.Request.QueryString.ToFacets();
			var matchingProducts = ProductIndex.Find<Ucommerce.Search.Models.Product>().Where(p => p.VariantSku == null);

			if (currentCategory != null)
			{
				matchingProducts = matchingProducts.Where(p => p.Categories.Contains(currentCategory.Guid));
			}

			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				matchingProducts = matchingProducts.Where(p => (
					 (p.Sku.Contains(searchTerm)
					  || p.Name.Contains(searchTerm)
					  || p.DisplayName.Contains(searchTerm)
					  || p.ShortDescription.Contains(searchTerm)
					  || p.LongDescription.Contains(searchTerm)
					 )
				 ));
			}

			return matchingProducts.ToList().AsQueryable();
		}

		private IList<ProductDTO> MapProducts(IList<Ucommerce.Search.Models.Product> products, Ucommerce.Search.Models.Category category, bool openInSamePage, Guid detailPageId)
		{
			var result = new List<ProductDTO>();
			var imageService = ObjectFactory.Instance.Resolve<IImageService>();
			var currentCatalog = CatalogContext.CurrentCatalog;
			var productsPrices = CatalogLibrary.CalculatePrices(products.Select(x => x.Guid).ToList());
			var productDefinitions = Ucommerce.EntitiesV2.ProductDefinition.All().ToList();

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

				var definition = productDefinitions.FirstOrDefault(pd => pd.Guid == product.ProductDefinition);

				var productViewModel = new ProductDTO()
				{
					Sku = product.Sku,
					VariantSku = product.VariantSku,
					Price = new Money(price, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString(),
					Discount = discount > 0 ? new Money(discount, CatalogContext.CurrentPriceGroup.CurrencyISOCode).ToString() : "",
					DisplayName = product.DisplayName,
					ThumbnailImageMediaUrl = product.ThumbnailImageUrl,
					ProductUrl = this.GetProductUrl(category, product, openInSamePage, detailPageId),
					IsSellableProduct = (definition.IsProductFamily() && product.ParentProduct != null) || (!definition.IsProductFamily() && product.ParentProduct == null),
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