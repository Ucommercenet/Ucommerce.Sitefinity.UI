using System;
using System.Collections.Generic;
using System.Linq;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Transactions;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Mini Basket MVC widget.
    /// </summary>
    public class MiniBasketModel : IMiniBasketModel
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private Guid cartPageId;
        private Guid productDetailsPageId;
        private Guid checkoutPageId;

        public MiniBasketModel(Guid? cartPageId = null, Guid? productDetailsPageId = null, Guid? checkoutPageId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            this.cartPageId = cartPageId ?? Guid.Empty;
            this.productDetailsPageId = productDetailsPageId ?? Guid.Empty;
            this.checkoutPageId = checkoutPageId ?? Guid.Empty;
        }

        public virtual MiniBasketRenderingViewModel CreateViewModel(string refreshUrl)
        {
            var viewModel = new MiniBasketRenderingViewModel();

            if (!_transactionLibraryInternal.HasBasket())
            {
                return viewModel;
            }

            PurchaseOrder basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
            viewModel.OrderLines = CartModel.GetOrlerLineList(basket, this.productDetailsPageId);

            viewModel.NumberOfItems = GetNumberOfItemsInBasket();
            viewModel.IsEmpty = IsBasketEmpty(viewModel);
            viewModel.Total = GetBasketTotal();
            viewModel.RefreshUrl = refreshUrl;
            viewModel.CartPageUrl = GettPageAbsoluteUrl(this.cartPageId);
            viewModel.CheckoutPageUrl = GettPageAbsoluteUrl(this.checkoutPageId);

            return viewModel;
        }

        public virtual MiniBasketRefreshViewModel Refresh()
        {
            var viewModel = new MiniBasketRefreshViewModel
            {
                IsEmpty = true
            };

            if (!_transactionLibraryInternal.HasBasket())
            {
                return viewModel;
            }

            var purchaseOrder = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
            
            viewModel.OrderLines = CartModel.GetOrlerLineList(purchaseOrder, this.productDetailsPageId);

            var quantity = purchaseOrder.OrderLines.Sum(x => x.Quantity);

            var total = purchaseOrder.OrderTotal.HasValue
                ? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency)
                : new Money(0, purchaseOrder.BillingCurrency);

            viewModel.NumberOfItems = quantity.ToString();
            viewModel.IsEmpty = quantity == 0;
            viewModel.Total = total.ToString();
            viewModel.CartPageUrl = GettPageAbsoluteUrl(cartPageId);
            viewModel.CheckoutPageUrl = GettPageAbsoluteUrl(this.checkoutPageId);

            return viewModel;
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

        private int GetNumberOfItemsInBasket()
        {
            if (_transactionLibraryInternal.HasBasket())
            {
                return _transactionLibraryInternal.GetBasket(false).PurchaseOrder.OrderLines.Sum(x => x.Quantity);
            }
            else
            {
                return 0;
            }
        }

        private string GettPageAbsoluteUrl(Guid pageId)
        {
            var pageUrl = Pages.UrlResolver.GetPageNodeUrl(pageId);

            return Pages.UrlResolver.GetAbsoluteUrl(pageUrl);
        }

        private bool IsBasketEmpty(MiniBasketRenderingViewModel model)
        {
            return model.NumberOfItems == 0;
        }

        private Money GetBasketTotal()
        {

            if (_transactionLibraryInternal.HasBasket())
            {
                var purchaseOrder = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

                if (purchaseOrder.OrderTotal.HasValue)
                {
                    return new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency);
                }
                else return new Money(0, purchaseOrder.BillingCurrency);
            }
            else return new Money(0, _transactionLibraryInternal.GetBasket(true).PurchaseOrder.BillingCurrency);
        }
    }
}
