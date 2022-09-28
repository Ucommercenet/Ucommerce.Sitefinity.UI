using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Content;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Slugs;
using UCommerce.Sitefinity.UI.Mvc.Services;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using Product = Ucommerce.Search.Models.Product;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Cart MVC widget.
    /// </summary>
    public class CartModel : ICartModel
    {
        private readonly Guid nextStepId;
        private readonly Guid productDetailsPageId;
        private readonly Guid redirectPageId;
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
        public IMarketingLibrary MarketingLibrary => ObjectFactory.Instance.Resolve<IMarketingLibrary>();
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        public CartModel(Guid? nextStepId = null, Guid? productDetailsPageId = null, Guid? redirectPageId = null)
        {
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.productDetailsPageId = productDetailsPageId ?? Guid.Empty;
            this.redirectPageId = redirectPageId ?? Guid.Empty;
        }

        public virtual CartUpdateBasketViewModel AddVoucher(CartUpdateBasket model)
        {
            var basket = TransactionLibrary.GetBasket();
            if (model.Vouchers.Any())
            {
                foreach (var modelVoucher in model.Vouchers)
                {
                    MarketingLibrary.AddVoucher(modelVoucher);
                    InsightUcommerce.SendOrderInteraction(basket, "Add voucher to cart", modelVoucher);
                }
            }

            TransactionLibrary.ExecuteBasketPipeline();
            var updatedBasket = MapCartUpdate(model);
            updatedBasket.Vouchers = model.Vouchers;

            return updatedBasket;
        }

        public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (SystemManager.IsDesignMode)
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

        public virtual CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl)
        {
            var basketVM = new CartRenderingViewModel();

            if (!TransactionLibrary.HasBasket())
            {
                return basketVM;
            }

            var basket = TransactionLibrary.GetBasket();
            basketVM.OrderLines = GetOrderLineList(basket, productDetailsPageId);

            GetDiscounts(basketVM, basket);
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
            basketVM.Discounts = basket.Discounts.Select(d => d.CampaignItemName)
                .ToList();

            InsightUcommerce.SendOrderInteraction(basket, "Checkout > View shopping cart", string.Empty);

            return basketVM;
        }

        public virtual CartUpdateBasketViewModel RemoveVoucher(CartUpdateBasket model)
        {
            var basket = TransactionLibrary.GetBasket();

            foreach (var voucher in model.Vouchers)
            {
                InsightUcommerce.SendOrderInteraction(basket, "Remove voucher from cart", voucher);

                var itemForDeletion = basket.Discounts.FirstOrDefault(d => d.CampaignItemName == voucher);

                if (itemForDeletion == null)
                {
                    continue;
                }

                basket.RemoveDiscount(itemForDeletion);

                var prop = basket.OrderProperties.FirstOrDefault(v => v.Key == "voucherCodes");
                if (prop == null)
                {
                    continue;
                }

                prop.Value = prop.Value.Replace(voucher + ",", string.Empty);
                prop.Save();
            }

            basket.Save();
            TransactionLibrary.ExecuteBasketPipeline();

            var updatedBasket = MapCartUpdate(model);
            updatedBasket.Vouchers.Except(model.Vouchers)
                .ToList();

            return updatedBasket;
        }

        public virtual CartUpdateBasketViewModel Update(CartUpdateBasket model)
        {
            foreach (var updateOrderline in model.RefreshBasket)
            {
                var orderLine = OrderLine.Get(updateOrderline.OrderLineId);
                var product = Ucommerce.EntitiesV2.Product.FirstOrDefault(p => p.Sku == orderLine.Sku && p.VariantSku == orderLine.VariantSku);
                var newQuantity = updateOrderline.OrderLineQty;
                if (newQuantity <= 0)
                {
                    newQuantity = 0;
                    InsightUcommerce.SendProductInteraction(product, "Remove product from cart", $"{product?.Name} ({product?.Sku})");
                }
                else
                {
                    InsightUcommerce.SendProductInteraction(product, "Change quantity of product in cart", $"{product.Name} ({product.Sku}) x{newQuantity}");
                }

                TransactionLibrary.UpdateLineItemByOrderLineId(updateOrderline.OrderLineId, newQuantity);
            }

            TransactionLibrary.ExecuteBasketPipeline();

            var updatedBasket = MapCartUpdate(model);

            return updatedBasket;
        }

        private void GetDiscounts(CartRenderingViewModel basketVM, PurchaseOrder basket)
        {
            foreach (var item in basket.Discounts)
            {
                if (!string.IsNullOrWhiteSpace(item.Description))
                {
                    if (item.Description.Contains(","))
                    {
                        basketVM.Discounts = item.Description.Split(',')
                            .ToList();
                    }
                    else
                    {
                        basketVM.Discounts.Add(item.Description);
                    }
                }
            }
        }

        private string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = UrlResolver.GetPageNodeUrl(nextStepId);

            return UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }

        private string GetRedirectUrl(Guid redirectPageId)
        {
            var redirectUrl = UrlResolver.GetPageNodeUrl(redirectPageId);

            return UrlResolver.GetAbsoluteUrl(redirectUrl);
        }

        private CartUpdateBasketViewModel MapCartUpdate(CartUpdateBasket model)
        {
            var basket = TransactionLibrary.GetBasket();
            var updatedBasket = MapOrderline(basket);

            var currencyIsoCode = basket.BillingCurrency.ISOCode;
            var orderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            var discountTotal = basket.DiscountTotal.GetValueOrDefault() > 0
                ? new Money(basket.DiscountTotal.GetValueOrDefault(), currencyIsoCode).ToString()
                : "";
            var taxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            var subTotal = new Money(basket.SubTotal.GetValueOrDefault(), currencyIsoCode).ToString();

            updatedBasket.OrderTotal = orderTotal;
            updatedBasket.DiscountTotal = discountTotal;
            updatedBasket.TaxTotal = taxTotal;
            updatedBasket.SubTotal = subTotal;
            updatedBasket.Vouchers.AddRange(model.Vouchers);

            return updatedBasket;
        }

        internal static IList<OrderlineViewModel> GetOrderLineList(PurchaseOrder basket, Guid productDetailsPageId)
        {
            var CatalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();

            var result = new List<OrderlineViewModel>();
            foreach (var orderLine in basket.OrderLines)
            {
                var product = CatalogLibrary.GetProduct(orderLine.Sku);
                ObjectFactory.Instance.Resolve<IImageService>();
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

        private static string GetProductUrl(Product product, Guid detailPageId)
        {
            var CatalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();
            var UrlService = ObjectFactory.Instance.Resolve<IUrlService>();

            if (detailPageId == Guid.Empty)
            {
                return UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
            }

            var baseUrl = UrlResolver.GetPageNodeUrl(detailPageId);

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

        private static CartUpdateBasketViewModel MapOrderline(PurchaseOrder basket)
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
    }
}
