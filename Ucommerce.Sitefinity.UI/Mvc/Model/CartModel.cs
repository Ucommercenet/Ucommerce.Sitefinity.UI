using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Ucommerce.Api;
using Ucommerce.Content;
using Ucommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce;
using Ucommerce.Search.Slugs;
using UCommerce.Sitefinity.UI.Mvc.Services;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Cart MVC widget.
	/// </summary>
	public class CartModel : ICartModel
	{
		private Guid productDetailsPageId;
		private Guid nextStepId;
		private Guid redirectPageId;
		public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
		public ITransactionLibrary TransactionLibrary => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<ITransactionLibrary>();
		public IMarketingLibrary MarketingLibrary => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<IMarketingLibrary>();
		public ICatalogContext CatalogContext => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<ICatalogContext>();
		public IUrlService UrlService => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<IUrlService>();

		public CartModel(Guid? nextStepId = null, Guid? productDetailsPageId = null, Guid? redirectPageId = null)
		{
			this.nextStepId = nextStepId ?? Guid.Empty;
			this.productDetailsPageId = productDetailsPageId ?? Guid.Empty;
			this.redirectPageId = redirectPageId ?? Guid.Empty;
		}

		public virtual CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl)
		{
			var basketVM = new CartRenderingViewModel();

			if (!TransactionLibrary.HasBasket())
			{
				return basketVM;
			}

			var basket = TransactionLibrary.GetBasket(false);
			basketVM.OrderLines = GetOrderLineList(basket, this.productDetailsPageId);

			this.GetDiscounts(basketVM, basket);
			var currencyIsoCode = basket.BillingCurrency.ISOCode;
			basketVM.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketVM.DiscountTotal = basket.DiscountTotal.GetValueOrDefault() > 0
				? new Money(basket.DiscountTotal.GetValueOrDefault(), currencyIsoCode).ToString()
				: "";
			basketVM.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketVM.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketVM.NextStepUrl = GetNextStepUrl(nextStepId);
			basketVM.RedirectUrl = GetRedirectUrl(redirectPageId);
			basketVM.RefreshUrl = refreshUrl;
			basketVM.RemoveOrderlineUrl = removeOrderLineUrl;
			basketVM.Discounts = basket.Discounts.Select(d => d.CampaignItemName).ToList();

			InsightUcommerce.SendOrderInteraction(basket, "Checkout > View shopping cart", string.Empty);

			return basketVM;
		}

		internal static IList<OrderlineViewModel> GetOrderLineList(Ucommerce.EntitiesV2.PurchaseOrder basket, Guid productDetailsPageId)
		{
			var CatalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();

			var result = new List<OrderlineViewModel>();
			foreach (var orderLine in basket.OrderLines)
			{
				var product = CatalogLibrary.GetProduct(orderLine.Sku);
				var imageService = ObjectFactory.Instance.Resolve<IImageService>();
				var currencyIsoCode = basket.BillingCurrency.ISOCode;
				var orderLineViewModel = new OrderlineViewModel
				{
					Quantity = orderLine.Quantity,
					ProductName = orderLine.ProductName,
					Sku = orderLine.Sku,
					VariantSku = orderLine.VariantSku,
					Total = new Money(orderLine.Total.GetValueOrDefault(), currencyIsoCode).ToString(),
					Discount = orderLine.Discount,
					Tax = new Money(orderLine.VAT, currencyIsoCode).ToString(),
					Price = new Money(orderLine.Price, currencyIsoCode).ToString(),
					ProductUrl = GetProductUrl(product, productDetailsPageId),
					PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), currencyIsoCode).ToString(),
					OrderLineId = orderLine.OrderLineId,
					ThumbnailName = product.Name,
					ThumbnailUrl = product.ThumbnailImageUrl
				};
				result.Add(orderLineViewModel);
			}
			return result;
		}

		private void GetDiscounts(CartRenderingViewModel basketVM, Ucommerce.EntitiesV2.PurchaseOrder basket)
		{
			foreach (var item in basket.Discounts)
			{
				if (!string.IsNullOrWhiteSpace(item.Description))
				{
					if (item.Description.Contains(","))
					{
						basketVM.Discounts = item.Description.Split(',').ToList();
					}
					else
					{
						basketVM.Discounts.Add(item.Description);
					}
				}
			}
		}

		public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
		{
			if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
			{
				message = "The widget is in Page Edit mode.";
				return false;
			}

			object submitModel = null;

			if (parameters.TryGetValue("submitModel", out submitModel))
			{
				var updateModel = submitModel as CartUpdateBasket;

				if (updateModel != null)
				{
					foreach (var item in updateModel.RefreshBasket)
					{
						if (item.OrderLineQty < 1)
						{
							message = string.Format("Quantity of {0} must be greater than 0", item.OrderLineId);
							return false;
						}
					}
				}
			}

			message = null;
			return true;
		}

		public virtual CartUpdateBasketViewModel Update(CartUpdateBasket model)
		{
			foreach (var updateOrderline in model.RefreshBasket)
			{
				var newQuantity = updateOrderline.OrderLineQty;
				if (newQuantity <= 0)
				{
					newQuantity = 0;
				}

				TransactionLibrary.UpdateLineItemByOrderLineId(updateOrderline.OrderLineId, newQuantity);
			}

			TransactionLibrary.ExecuteBasketPipeline();

			var updatedBasket = MapCartUpdate(model);

			return updatedBasket;
		}

		public virtual CartUpdateBasketViewModel RemoveVoucher(CartUpdateBasket model)
		{
			var basket = TransactionLibrary.GetBasket(false);

			foreach (var item in model.Vouchers)
			{
				var itemForDeletion = basket.Discounts.FirstOrDefault(d => d.CampaignItemName == item);

				if (itemForDeletion != null)
				{
					basket.RemoveDiscount(itemForDeletion);
					var prop = basket.OrderProperties.FirstOrDefault(v => v.Key == "voucherCodes");
					if (prop != null)
					{
						prop.Value = prop.Value.Replace(item + ",", string.Empty);
						prop.Save();
					}
				}
			}

			basket.Save();
			TransactionLibrary.ExecuteBasketPipeline();

			var updatedBasket = MapCartUpdate(model);
			updatedBasket.Vouchers.Except(model.Vouchers).ToList();

			return updatedBasket;
		}

		public virtual CartUpdateBasketViewModel AddVoucher(CartUpdateBasket model)
		{
			if (model.Vouchers.Any())
			{
				foreach (var modelVoucher in model.Vouchers)
				{
					MarketingLibrary.AddVoucher(modelVoucher);
				}
			}

			TransactionLibrary.ExecuteBasketPipeline();
			var updatedBasket = MapCartUpdate(model);
			updatedBasket.Vouchers = model.Vouchers;

			return updatedBasket;
		}

		private static CartUpdateBasketViewModel MapOrderline(Ucommerce.EntitiesV2.PurchaseOrder basket)
		{
			var updatedBasket = new CartUpdateBasketViewModel();

			foreach (var orderLine in basket.OrderLines)
			{
				var currencyIsoCode = basket.BillingCurrency.ISOCode;
				var orderLineViewModel = new CartUpdateOrderline();
				orderLineViewModel.OrderlineId = orderLine.OrderLineId;
				orderLineViewModel.Quantity = orderLine.Quantity;
				orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), currencyIsoCode).ToString();
				orderLineViewModel.Discount = orderLine.Discount;
				orderLineViewModel.Tax = new Money(orderLine.VAT, currencyIsoCode).ToString();
				orderLineViewModel.Price = new Money(orderLine.Price, currencyIsoCode).ToString();
				orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, currencyIsoCode).ToString();

				updatedBasket.OrderLines.Add(orderLineViewModel);
			}

			return updatedBasket;
		}

		private CartUpdateBasketViewModel MapCartUpdate(CartUpdateBasket model)
		{
			var basket = TransactionLibrary.GetBasket(false);
			var updatedBasket = MapOrderline(basket);

			var currencyIsoCode = basket.BillingCurrency.ISOCode;
			string orderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			string discountTotal = basket.DiscountTotal.GetValueOrDefault() > 0
										? new Money(basket.DiscountTotal.GetValueOrDefault(), currencyIsoCode).ToString()
										: "";
			string taxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			string subTotal = new Money(basket.SubTotal.GetValueOrDefault(), currencyIsoCode).ToString();

			updatedBasket.OrderTotal = orderTotal;
			updatedBasket.DiscountTotal = discountTotal;
			updatedBasket.TaxTotal = taxTotal;
			updatedBasket.SubTotal = subTotal;
			updatedBasket.Vouchers.AddRange(model.Vouchers);

			return updatedBasket;
		}

		private string GetNextStepUrl(Guid nextStepId)
		{
			var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

			return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
		}

		private string GetRedirectUrl(Guid redirectPageId)
		{
			var redirectUrl = Pages.UrlResolver.GetPageNodeUrl(redirectPageId);

			return Pages.UrlResolver.GetAbsoluteUrl(redirectUrl);
		}

		private static string GetProductUrl(Ucommerce.Search.Models.Product product, Guid detailPageId)
		{
			var CatalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();
			var UrlService = ObjectFactory.Instance.Resolve<IUrlService>();

			if (detailPageId == Guid.Empty)
			{
				return UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
			}

			var baseUrl = Pages.UrlResolver.GetPageNodeUrl(detailPageId);

			string catUrl;
			var productCategory = product.Categories.FirstOrDefault();
			if (productCategory == null)
			{
				catUrl = CategoryModel.DefaultCategoryName;
			}
			else
			{
				var category = CatalogContext.CurrentCategories.FirstOrDefault(x => x.Guid == productCategory);
				catUrl = CategoryModel.GetCategoryPath(category);
			}

			var rawtUrl = string.Format("{0}/{1}", catUrl, product.Slug);
			string relativeUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", rawtUrl);

			string url;

			if (SystemManager.CurrentHttpContext.Request.Url != null)
			{
				url = UrlPath.ResolveUrl(relativeUrl, true);
			}
			else
			{
				url = Pages.UrlResolver.GetAbsoluteUrl(relativeUrl);
			}

			return url;
		}
	}
}