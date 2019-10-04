using System;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class OrderOverviewModel : IOrderOverviewModel
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private Guid nextStepId;

        public OrderOverviewModel(Guid? nextStepId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            this.nextStepId = nextStepId ?? Guid.Empty;
        }

        public OrderOverviewViewModel GetViewModel()
        {
            var orderVM = new OrderOverviewViewModel();

            if (!_transactionLibraryInternal.HasBasket())
            {
                return orderVM;
            }

            PurchaseOrder basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;

            foreach (var orderLine in basket.OrderLines)
            {
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
                    ProductUrl = CatalogLibrary.GetNiceUrlForProduct(CatalogLibrary.GetProduct(orderLine.Sku)),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), basket.BillingCurrency).ToString(),
                    OrderLineId = orderLine.OrderLineId
                };
                orderVM.OrderLines.Add(orderLineViewModel);
            }

            orderVM.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            orderVM.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            orderVM.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            orderVM.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            orderVM.NextStepUrl = GetNextStepUrl(nextStepId);

            return orderVM;
        }

        private string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }
    }
}
