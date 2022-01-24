using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Ucommerce.Infrastructure;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce.Api;
using Ucommerce;
using UCommerce.Sitefinity.UI.Mvc.Services;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Basket Preview MVC widget.
	/// </summary>
	public class BasketPreviewModel : IBasketPreviewModel
	{
		private Guid nextStepId;
		private Guid previousStepId;
		public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        public BasketPreviewModel(Guid? nextStepId = null, Guid? previousStepId = null)
		{
			this.nextStepId = nextStepId ?? Guid.Empty;
			this.previousStepId = previousStepId ?? Guid.Empty;
		}

		public virtual BasketPreviewViewModel GetViewModel()
		{
			var purchaseOrder = TransactionLibrary.GetBasket(false);
			var basketPreviewViewModel = new BasketPreviewViewModel();
			MapAddresses(purchaseOrder, basketPreviewViewModel);

			foreach (var orderLine in purchaseOrder.OrderLines)
			{
                var orderLineCurrencyIsoCode = orderLine.PurchaseOrder.BillingCurrency.ISOCode;
                var orderLineModel = new PreviewOrderLine
				{
					ProductName = orderLine.ProductName,
					Sku = orderLine.Sku,
					VariantSku = orderLine.VariantSku,
					Total = new Money(orderLine.Total.GetValueOrDefault(), orderLineCurrencyIsoCode).ToString(),
					Tax = new Money(orderLine.VAT, orderLineCurrencyIsoCode).ToString(),
					Price = new Money(orderLine.Price, orderLineCurrencyIsoCode).ToString(),
					PriceWithDiscount = new Money(orderLine.Price - orderLine.UnitDiscount.GetValueOrDefault(),
                        orderLineCurrencyIsoCode).ToString(),
					Quantity = orderLine.Quantity,
					Discount = orderLine.Discount
				};

				basketPreviewViewModel.OrderLines.Add(orderLineModel);
			}

            var currencyIsoCode = purchaseOrder.BillingCurrency.ISOCode;

            basketPreviewViewModel.DiscountTotal =
				new Money(purchaseOrder.DiscountTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketPreviewViewModel.DiscountAmount = purchaseOrder.DiscountTotal.GetValueOrDefault();
			basketPreviewViewModel.SubTotal =
				new Money(purchaseOrder.SubTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketPreviewViewModel.OrderTotal =
				new Money(purchaseOrder.OrderTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketPreviewViewModel.TaxTotal =
				new Money(purchaseOrder.TaxTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketPreviewViewModel.ShippingTotal =
				new Money(purchaseOrder.ShippingTotal.GetValueOrDefault(), currencyIsoCode).ToString();
			basketPreviewViewModel.PaymentTotal =
				new Money(purchaseOrder.PaymentTotal.GetValueOrDefault(), currencyIsoCode).ToString();

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

			InsightUcommerce.SendInteraction(purchaseOrder, "Checkout", "Order Preview");

			return basketPreviewViewModel;
		}

		private static void MapAddresses(Ucommerce.EntitiesV2.PurchaseOrder purchaseOrder, BasketPreviewViewModel basketPreviewViewModel)
		{
			if (purchaseOrder.BillingAddress != null)
			{
				if (!purchaseOrder.BillingAddress.FirstName.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.FirstName = purchaseOrder.BillingAddress.FirstName;
				}

				if (!purchaseOrder.BillingAddress.LastName.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.LastName = purchaseOrder.BillingAddress.LastName;
				}

				if (!purchaseOrder.BillingAddress.EmailAddress.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.EmailAddress = purchaseOrder.BillingAddress.EmailAddress;
				}

				if (!purchaseOrder.BillingAddress.PhoneNumber.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.PhoneNumber = purchaseOrder.BillingAddress.PhoneNumber;
				}

				if (!purchaseOrder.BillingAddress.MobilePhoneNumber.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.MobilePhoneNumber = purchaseOrder.BillingAddress.MobilePhoneNumber;
				}

				if (!purchaseOrder.BillingAddress.Line1.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.Line1 = purchaseOrder.BillingAddress.Line1;
				}

				if (!purchaseOrder.BillingAddress.Line2.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.Line2 = purchaseOrder.BillingAddress.Line2;
				}

				if (!purchaseOrder.BillingAddress.PostalCode.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.PostalCode = purchaseOrder.BillingAddress.PostalCode;
				}

				if (!purchaseOrder.BillingAddress.City.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.City = purchaseOrder.BillingAddress.City;
				}

				if (!purchaseOrder.BillingAddress.State.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.State = purchaseOrder.BillingAddress.State;
				}

				if (!purchaseOrder.BillingAddress.Attention.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.Attention = purchaseOrder.BillingAddress.Attention;
				}

				if (!purchaseOrder.BillingAddress.CompanyName.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.CompanyName = purchaseOrder.BillingAddress.CompanyName;
				}

				if (purchaseOrder.BillingAddress.Country.CountryId > 0)
				{
					basketPreviewViewModel.BillingAddress.Country.CountryId = purchaseOrder.BillingAddress.Country.CountryId;
				}

				if (!purchaseOrder.BillingAddress.Country.Name.IsNullOrWhitespace())
				{
					basketPreviewViewModel.BillingAddress.Country.Name = purchaseOrder.BillingAddress.Country.Name;
				}
			}

			var shipmentAddress = purchaseOrder.GetShippingAddress(Ucommerce.Constants.DefaultShipmentAddressName);

			if (shipmentAddress != null)
			{

				if (!shipmentAddress.FirstName.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.FirstName = shipmentAddress.FirstName;
				}

				if (!shipmentAddress.LastName.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.LastName = shipmentAddress.LastName;
				}

				if (!shipmentAddress.EmailAddress.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.EmailAddress = shipmentAddress.EmailAddress;
				}

				if (!shipmentAddress.PhoneNumber.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.PhoneNumber = shipmentAddress.PhoneNumber;
				}

				if (!shipmentAddress.MobilePhoneNumber.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.MobilePhoneNumber = shipmentAddress.MobilePhoneNumber;
				}

				if (!shipmentAddress.Line1.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.Line1 = shipmentAddress.Line1;
				}

				if (!shipmentAddress.Line2.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.Line2 = shipmentAddress.Line2;
				}

				if (!shipmentAddress.PostalCode.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.PostalCode = shipmentAddress.PostalCode;
				}

				if (!shipmentAddress.City.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.City = shipmentAddress.City;
				}

				if (!shipmentAddress.State.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.State = shipmentAddress.State;
				}

				if (!shipmentAddress.Attention.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.Attention = shipmentAddress.Attention;
				}

				if (!shipmentAddress.CompanyName.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.CompanyName = shipmentAddress.CompanyName;
				}

				if (shipmentAddress.Country.CountryId > 0)
				{
					basketPreviewViewModel.ShipmentAddress.Country.CountryId = shipmentAddress.Country.CountryId;
				}

				if (!shipmentAddress.Country.Name.IsNullOrWhitespace())
				{
					basketPreviewViewModel.ShipmentAddress.Country.Name = shipmentAddress.Country.Name;
				}
			}
		}

		public virtual string GetPaymentUrl()
		{
			var payment = TransactionLibrary.GetBasket().Payments.First();
			if (payment.PaymentMethod.PaymentMethodServiceName == null)
			{
				return "/confirmation";
			}

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			string paymentUrl = TransactionLibrary.GetPaymentPageUrl(payment);
			return paymentUrl;
		}

		public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
		{
			object mode = null;

			if (parameters.TryGetValue("mode", out mode) && mode != null)
			{
				if (mode.ToString() == "index")
				{
					if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
					{
						message = "The widget is in Page Edit mode.";
						return false;
					}
				}

				message = null;
				return true;
			}
			else
			{
				Ucommerce.EntitiesV2.PurchaseOrder purchaseOrder;
				try
				{
					purchaseOrder = TransactionLibrary.GetBasket(false);
				}
				catch
				{
					message = "The checkout has not started yet";
					return false;
				}

				if (purchaseOrder.BillingAddress == null)
				{
					message = "The Billing Address must be specified.";
					return false;
				}

				if (purchaseOrder.GetShippingAddress(Ucommerce.Constants.DefaultShipmentAddressName) == null)
				{
					message = "The Shipping Address must be specified.";
					return false;
				}

				message = null;
				return true;
			}
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