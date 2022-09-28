using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Services;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Mini Basket MVC widget.
    /// </summary>
    public class MiniBasketModel : IMiniBasketModel
    {
        private readonly Guid cartPageId;
        private readonly Guid checkoutPageId;
        private readonly Guid productDetailsPageId;
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        public MiniBasketModel(Guid? cartPageId = null, Guid? productDetailsPageId = null, Guid? checkoutPageId = null)
        {
            this.cartPageId = cartPageId ?? Guid.Empty;
            this.productDetailsPageId = productDetailsPageId ?? Guid.Empty;
            this.checkoutPageId = checkoutPageId ?? Guid.Empty;
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

        public virtual MiniBasketRenderingViewModel CreateViewModel(string refreshUrl)
        {
            var viewModel = new MiniBasketRenderingViewModel();

            if (!TransactionLibrary.HasBasket())
            {
                return viewModel;
            }

            var basket = TransactionLibrary.GetBasket();
            viewModel.OrderLines = CartModel.GetOrderLineList(basket, productDetailsPageId);

            viewModel.NumberOfItems = GetNumberOfItemsInBasket();
            viewModel.IsEmpty = IsBasketEmpty(viewModel);
            viewModel.Total = GetBasketTotal();
            viewModel.RefreshUrl = refreshUrl;
            viewModel.CartPageUrl = GetPageAbsoluteUrl(cartPageId);
            viewModel.CheckoutPageUrl = GetPageAbsoluteUrl(checkoutPageId);

            return viewModel;
        }

        public virtual MiniBasketRefreshViewModel Refresh()
        {
            var viewModel = new MiniBasketRefreshViewModel
            {
                IsEmpty = true
            };

            if (!TransactionLibrary.HasBasket())
            {
                return viewModel;
            }

            var purchaseOrder = TransactionLibrary.GetBasket();

            viewModel.OrderLines = CartModel.GetOrderLineList(purchaseOrder, productDetailsPageId);

            var quantity = purchaseOrder.OrderLines.Sum(x => x.Quantity);

            var currencyIsoCode = purchaseOrder.BillingCurrency.ISOCode;
            var total = purchaseOrder.OrderTotal.HasValue
                ? new Money(purchaseOrder.OrderTotal.Value, currencyIsoCode)
                : new Money(0, currencyIsoCode);

            viewModel.NumberOfItems = quantity.ToString();
            viewModel.IsEmpty = quantity == 0;
            viewModel.Total = total.ToString();
            viewModel.CartPageUrl = GetPageAbsoluteUrl(cartPageId);
            viewModel.CheckoutPageUrl = GetPageAbsoluteUrl(checkoutPageId);
            viewModel.DiscountTotal = purchaseOrder.DiscountTotal.GetValueOrDefault() > 0
                ? new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), currencyIsoCode).ToString()
                : "";
            viewModel.TaxTotal = new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            viewModel.SubTotal = new Money(purchaseOrder.SubTotal.GetValueOrDefault(), currencyIsoCode).ToString();

            return viewModel;
        }

        private Money GetBasketTotal()
        {
            if (TransactionLibrary.HasBasket())
            {
                var purchaseOrder = TransactionLibrary.GetBasket();

                if (purchaseOrder.OrderTotal.HasValue)
                {
                    return new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency.ISOCode);
                }

                return new Money(0, purchaseOrder.BillingCurrency.ISOCode);
            }

            return new Money(0,
                TransactionLibrary.GetBasket(true)
                    .BillingCurrency.ISOCode);
        }

        private int GetNumberOfItemsInBasket()
        {
            if (TransactionLibrary.HasBasket())
            {
                return TransactionLibrary.GetBasket()
                    .OrderLines.Sum(x => x.Quantity);
            }

            return 0;
        }

        private string GetPageAbsoluteUrl(Guid pageId)
        {
            var pageUrl = UrlResolver.GetPageNodeUrl(pageId);

            return UrlResolver.GetAbsoluteUrl(pageUrl);
        }

        private bool IsBasketEmpty(MiniBasketRenderingViewModel model)
        {
            return model.NumberOfItems == 0;
        }
    }
}
