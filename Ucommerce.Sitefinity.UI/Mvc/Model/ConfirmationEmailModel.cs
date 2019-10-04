using System;
using System.Linq;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class ConfirmationEmailModel
    {
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;

        public ConfirmationEmailModel()
        {
            _purchaseOrderRepository = ObjectFactory.Instance.Resolve<IRepository<PurchaseOrder>>();
        }

        public ConfirmationEmailViewModel GetViewModel(string orderGuid)
        {
            var confirmationEmailViewModel = new ConfirmationEmailViewModel();

            if (!string.IsNullOrEmpty(orderGuid))
            {
                var purchaseOrder = _purchaseOrderRepository.SingleOrDefault(x => x.OrderGuid == new Guid(orderGuid));
                confirmationEmailViewModel = MapPurchaseOrderToViewModel(purchaseOrder, confirmationEmailViewModel);
            }
            return confirmationEmailViewModel;
        }

        private ConfirmationEmailViewModel MapPurchaseOrderToViewModel(PurchaseOrder purchaseOrder, ConfirmationEmailViewModel confirmationEmailViewModel)
        {
            confirmationEmailViewModel.BillingAddress = purchaseOrder.BillingAddress ?? new OrderAddress();
            confirmationEmailViewModel.ShipmentAddress = purchaseOrder.GetShippingAddress("Default") ?? new OrderAddress();

            foreach (var orderLine in purchaseOrder.OrderLines)
            {
                var orderLineModel = new ConfirmationEmailOrderLine
                {
                    ProductName = orderLine.ProductName,
                    Sku = orderLine.Sku,
                    VariantSku = orderLine.VariantSku,
                    Total = new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency).ToString(),
                    Tax = new Money(orderLine.VAT, purchaseOrder.BillingCurrency).ToString(),
                    Price = new Money(orderLine.Price, purchaseOrder.BillingCurrency).ToString(),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, purchaseOrder.BillingCurrency).ToString(),
                    Quantity = orderLine.Quantity,
                    Discount = orderLine.Discount
                };
                confirmationEmailViewModel.OrderLines.Add(orderLineModel);
            }

            confirmationEmailViewModel.DiscountTotal = new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.DiscountAmount = purchaseOrder.DiscountTotal.GetValueOrDefault();
            confirmationEmailViewModel.SubTotal = new Money(purchaseOrder.SubTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.OrderTotal = new Money(purchaseOrder.OrderTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.TaxTotal = new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();
            confirmationEmailViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), purchaseOrder.BillingCurrency).ToString();

            confirmationEmailViewModel.OrderNumber = purchaseOrder.OrderNumber;
            confirmationEmailViewModel.CustomerName = purchaseOrder.Customer.FirstName;

            var shipment = purchaseOrder.Shipments.FirstOrDefault();
            if (shipment != null)
            {
                confirmationEmailViewModel.ShipmentName = shipment.ShipmentName;
                confirmationEmailViewModel.ShipmentAmount = purchaseOrder.ShippingTotal.GetValueOrDefault();
            }

            var payment = purchaseOrder.Payments.FirstOrDefault();
            if (payment != null)
            {
                confirmationEmailViewModel.PaymentName = payment.PaymentMethodName;
                confirmationEmailViewModel.PaymentAmount = purchaseOrder.PaymentTotal.GetValueOrDefault();
            }

            return confirmationEmailViewModel;
        }
    }
}
