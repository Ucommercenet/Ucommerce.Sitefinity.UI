using System;
using System.Collections.Generic;
using System.Linq;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce;
using Ucommerce.EntitiesV2;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Confirmation Email MVC widget.
    /// </summary>
    public class ConfirmationEmailModel: IConfirmationEmailModel
    {
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;

        public ConfirmationEmailModel()
        {
            _purchaseOrderRepository = Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<IRepository<PurchaseOrder>>();
        }

        public virtual ConfirmationEmailViewModel GetViewModel(string orderGuid)
        {
            var confirmationEmailViewModel = new ConfirmationEmailViewModel();

            if (!string.IsNullOrWhiteSpace(orderGuid))
            {
                var purchaseOrder = _purchaseOrderRepository.SingleOrDefault(x => x.OrderGuid == new Guid(orderGuid));
                confirmationEmailViewModel = MapPurchaseOrder(purchaseOrder, confirmationEmailViewModel);
            }
            return confirmationEmailViewModel;
        }

        public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            if (parameters.ContainsKey("orderGuid"))
            {
                var orderGuid = parameters["orderGuid"] as string;
                if (string.IsNullOrWhiteSpace(orderGuid))
                {
                    message = null;
                    return false;
                }
            }

            message = null;
            return true;
        }

        private ConfirmationEmailViewModel MapPurchaseOrder(PurchaseOrder purchaseOrder, ConfirmationEmailViewModel confirmationEmailViewModel)
        {
            confirmationEmailViewModel.BillingAddress = purchaseOrder.BillingAddress ?? new OrderAddress();
            confirmationEmailViewModel.ShipmentAddress = purchaseOrder.GetShippingAddress("Shipment") ?? new OrderAddress();
            
            foreach (var orderLine in purchaseOrder.OrderLines)
            {
                var orderLineCurrencyIsoCode = orderLine.PurchaseOrder.BillingCurrency.ISOCode;
                var orderLineModel = new ConfirmationEmailOrderLine
                {
                    ProductName = orderLine.ProductName,
                    Sku = orderLine.Sku,
                    VariantSku = orderLine.VariantSku,
                    Total = new Money(orderLine.Total.GetValueOrDefault(), orderLineCurrencyIsoCode).ToString(),
                    Tax = new Money(orderLine.VAT, orderLineCurrencyIsoCode).ToString(),
                    Price = new Money(orderLine.Price, orderLineCurrencyIsoCode).ToString(),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, orderLineCurrencyIsoCode).ToString(),
                    Quantity = orderLine.Quantity,
                    Discount = orderLine.Discount
                };
                confirmationEmailViewModel.OrderLines.Add(orderLineModel);
            }

            var currencyIsoCode = purchaseOrder.BillingCurrency.ISOCode;
            confirmationEmailViewModel.DiscountTotal = new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            confirmationEmailViewModel.DiscountAmount = purchaseOrder.DiscountTotal.GetValueOrDefault();
            confirmationEmailViewModel.SubTotal = new Money(purchaseOrder.SubTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            confirmationEmailViewModel.OrderTotal = new Money(purchaseOrder.OrderTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            confirmationEmailViewModel.TaxTotal = new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            confirmationEmailViewModel.ShippingTotal = new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), currencyIsoCode).ToString();
            confirmationEmailViewModel.PaymentTotal = new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), currencyIsoCode).ToString();

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
