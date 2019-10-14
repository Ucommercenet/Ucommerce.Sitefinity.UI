using System;
using System.Linq;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class BasketPreviewModel : IBasketPreviewModel
    {
        private Guid nextStepId;
        private Guid previousStepId;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public BasketPreviewModel(Guid? nextStepId = null, Guid? previousStepId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.previousStepId = previousStepId ?? Guid.Empty;
        }

        public BasketPreviewViewModel GetViewModelr(PurchaseOrder purchaseOrder)
        {
            var basketPreviewViewModel = new BasketPreviewViewModel();
            basketPreviewViewModel.BillingAddress = purchaseOrder.BillingAddress ?? new OrderAddress();
            basketPreviewViewModel.ShipmentAddress = purchaseOrder.GetShippingAddress(UCommerce.Constants.DefaultShipmentAddressName) ?? new OrderAddress();

            foreach (var orderLine in purchaseOrder.OrderLines)
            {
                var orderLineModel = new PreviewOrderLine
                {
                    ProductName = orderLine.ProductName,
                    Sku = orderLine.Sku,
                    VariantSku = orderLine.VariantSku,
                    Total = new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency).ToString(),
                    Tax = new Money(orderLine.VAT, purchaseOrder.BillingCurrency).ToString(),
                    Price = new Money(orderLine.Price, purchaseOrder.BillingCurrency).ToString(),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString(),
                    Quantity = orderLine.Quantity,
                    Discount = orderLine.Discount
                };

                basketPreviewViewModel.OrderLines.Add(orderLineModel);
            }

            basketPreviewViewModel.DiscountTotal = new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            basketPreviewViewModel.DiscountAmount = purchaseOrder.DiscountTotal.GetValueOrDefault();
            basketPreviewViewModel.SubTotal = new Money(purchaseOrder.SubTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            basketPreviewViewModel.OrderTotal = new Money(purchaseOrder.OrderTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            basketPreviewViewModel.TaxTotal = new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            basketPreviewViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            basketPreviewViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();

            var shipment = purchaseOrder.Shipments.FirstOrDefault();
            if (shipment != null)
            {
                basketPreviewViewModel.ShipmentName = shipment.ShipmentName;
                basketPreviewViewModel.ShipmentAmount = purchaseOrder.ShippingTotal.GetValueOrDefault();
            }

            var payment = purchaseOrder.Payments.FirstOrDefault();
            if (payment != null)
            {
                basketPreviewViewModel.PaymentName = payment.PaymentMethodName;
                basketPreviewViewModel.PaymentAmount = purchaseOrder.PaymentTotal.GetValueOrDefault();
            }
            basketPreviewViewModel.NextStepUrl = GetNextStepUrl(nextStepId, purchaseOrder.OrderGuid);
            basketPreviewViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

            return basketPreviewViewModel;
        }

        private string GetNextStepUrl(Guid nextStepId, Guid orderGuid)
        {
            var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);
            var pageUrl = Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
            pageUrl += "?orderGuid=" + orderGuid.ToString();
            return pageUrl;
        }

        private string GetPreviousStepUrl(Guid previousStepId)
        {
            var previousStepUrl = Pages.UrlResolver.GetPageNodeUrl(previousStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(previousStepUrl);
        }
    }
}
