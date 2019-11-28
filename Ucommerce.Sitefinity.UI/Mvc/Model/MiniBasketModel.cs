using System;
using System.Collections.Generic;
using System.Linq;
using UCommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public class MiniBasketModel : IMiniBasketModel
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private Guid cartPageId;

        public MiniBasketModel(Guid? cartPageId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            this.cartPageId = cartPageId ?? Guid.Empty;
        }

        public virtual MiniBasketRenderingViewModel CreateViewModel(string refreshUrl)
        {
            var viewModel = new MiniBasketRenderingViewModel();

            viewModel.NumberOfItems = GetNumberOfItemsInBasket();
            viewModel.IsEmpty = IsBasketEmpty(viewModel);
            viewModel.Total = GetBasketTotal();
            viewModel.RefreshUrl = refreshUrl;
            viewModel.CartPageUrl = GetCartPageAbsoluteUrl(cartPageId);

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

        private string GetCartPageAbsoluteUrl(Guid cartPageId)
        {
            var cartPageUrl = Pages.UrlResolver.GetPageNodeUrl(cartPageId);

            return Pages.UrlResolver.GetAbsoluteUrl(cartPageUrl);
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
