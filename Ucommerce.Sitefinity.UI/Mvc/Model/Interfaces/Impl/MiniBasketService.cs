using System.Linq;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces.Impl
{
    class MiniBasketService : IMiniBasketService
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public MiniBasketService()
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>(); ;
        }

        public MiniBasketRefreshViewModel Refresh()
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

            var quantity = purchaseOrder.OrderLines.Sum(x => x.Quantity);

            var total = purchaseOrder.OrderTotal.HasValue
                ? new Money(purchaseOrder.OrderTotal.Value, purchaseOrder.BillingCurrency)
                : new Money(0, purchaseOrder.BillingCurrency);

            viewModel.NumberOfItems = quantity.ToString();
            viewModel.IsEmpty = quantity == 0;
            viewModel.Total = total.ToString();

            return viewModel;
        }
    }
}
