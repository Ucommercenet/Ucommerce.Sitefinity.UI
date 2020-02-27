using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Transactions;

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
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public CartModel(Guid? nextStepId = null, Guid? productDetailsPageId = null, Guid? redirectPageId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.productDetailsPageId = productDetailsPageId ?? Guid.Empty;
            this.redirectPageId = redirectPageId ?? Guid.Empty;
        }

        public virtual CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl)
        {
            var basketVM = new CartRenderingViewModel();

            if (!_transactionLibraryInternal.HasBasket())
            {
                return basketVM;
            }

            PurchaseOrder basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
            foreach (var orderLine in basket.OrderLines)
            {
                var product = CatalogLibrary.GetProduct(orderLine.Sku);
                var imageService = UCommerce.Infrastructure.ObjectFactory.Instance.Resolve<IImageService>();
                var orderLineViewModel = new OrderlineViewModel
                {
                    Quantity = orderLine.Quantity,
                    ProductName = orderLine.ProductName,
                    Sku = orderLine.Sku,
                    VariantSku = orderLine.VariantSku,
                    Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString(),
                    Discount = orderLine.Discount,
                    Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString(),
                    Price = new Money(orderLine.Price, basket.BillingCurrency).ToString(),
                    ProductUrl = GetProductUrl(CatalogLibrary.GetProduct(orderLine.Sku), this.productDetailsPageId),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), basket.BillingCurrency).ToString(),
                    OrderLineId = orderLine.OrderLineId,
                    ThumbnailName = imageService.GetImage(product.ThumbnailImageMediaId).Name,
                    ThumbnailUrl = imageService.GetImage(product.ThumbnailImageMediaId).Url
                };
                basketVM.OrderLines.Add(orderLineViewModel);
            }

            basketVM.Discounts = basket.Discounts.Select(d => d.Description).ToList();
            basketVM.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketVM.DiscountTotal = basket.DiscountTotal.GetValueOrDefault() > 0 ? new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString() : "";
            basketVM.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketVM.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketVM.NextStepUrl = GetNextStepUrl(nextStepId);
            basketVM.RedirectUrl = GetRedirectUrl(redirectPageId);
            basketVM.RefreshUrl = refreshUrl;
            basketVM.RemoveOrderlineUrl = removeOrderLineUrl;

            return basketVM;
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

                _transactionLibraryInternal.UpdateLineItemByOrderLineId(updateOrderline.OrderLineId, newQuantity);
            }

            MarketingLibrary.AddVoucher(model.Voucher);
            _transactionLibraryInternal.ExecuteBasketPipeline();

            var basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

            CartUpdateBasketViewModel updatedBasket = new CartUpdateBasketViewModel();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineViewModel = new CartUpdateOrderline();
                orderLineViewModel.OrderlineId = orderLine.OrderLineId;
                orderLineViewModel.Quantity = orderLine.Quantity;
                orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString();
                orderLineViewModel.Discount = orderLine.Discount;
                orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString();
                orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency).ToString();
                orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency).ToString();

                updatedBasket.OrderLines.Add(orderLineViewModel);
            }

            string orderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string discountTotal = basket.DiscountTotal.GetValueOrDefault() > 0 ? new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString() : "";
            string taxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string subTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            string voucher = "";

            if (basket.Discounts.FirstOrDefault(d => d.Description == model.Voucher) != null)
            {
                voucher = model.Voucher;
            }

            updatedBasket.OrderTotal = orderTotal;
            updatedBasket.DiscountTotal = discountTotal;
            updatedBasket.TaxTotal = taxTotal;
            updatedBasket.SubTotal = subTotal;
            updatedBasket.Voucher = voucher;

            return updatedBasket;
        }


        private string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }

        private string GetProductUrl(Product product, Guid detailPageId)
        {
            if (detailPageId == Guid.Empty)
            {
                return CatalogLibrary.GetNiceUrlForProduct(product);
            }

            var baseUrl = UCommerce.Sitefinity.UI.Pages.UrlResolver.GetPageNodeUrl(detailPageId);

            string catUrl;
            var productCategory = product.GetCategories().FirstOrDefault();
            if (productCategory == null)
            {
                catUrl = CategoryModel.DefaultCategoryName;
            }
            else
            {
                catUrl = CategoryModel.GetCategoryPath(productCategory);
            }

            var rawtUrl = string.Format("{0}/{1}", catUrl, product.ProductId);
            string relativeUrl = string.Concat(VirtualPathUtility.RemoveTrailingSlash(baseUrl), "/", rawtUrl);

            string url;

            if (SystemManager.CurrentHttpContext.Request.Url != null)
            {
                url = UrlPath.ResolveUrl(relativeUrl, true);
            }
            else
            {
                url = UCommerce.Sitefinity.UI.Pages.UrlResolver.GetAbsoluteUrl(relativeUrl);
            }

            return url;
        }
    }
}