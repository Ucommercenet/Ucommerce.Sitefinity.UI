using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Ucommerce.Api;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using Ucommerce;
using UCommerce.Sitefinity.UI.Mvc.Services;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
	/// <summary>
	/// The Model class of the Payment Picker MVC widget.
	/// </summary>
	public class PaymentPickerModel : IPaymentPickerModel
	{
		private Guid nextStepId;
		private Guid previousStepId;
		public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
		public ITransactionLibrary TransactionLibrary => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<ITransactionLibrary>();
		public ICatalogContext CatalogContext => Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<ICatalogContext>();
		public Ucommerce.EntitiesV2.IRepository<Ucommerce.EntitiesV2.PriceGroup> PriceGroupRepository = Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<Ucommerce.EntitiesV2.IRepository<Ucommerce.EntitiesV2.PriceGroup>>();

		public PaymentPickerModel(Guid? nextStepId = null, Guid? previousStepId = null)
		{
			this.nextStepId = nextStepId ?? Guid.Empty;
			this.previousStepId = previousStepId ?? Guid.Empty;
		}

		public virtual PaymentPickerViewModel GetViewModel()
		{
			Ucommerce.EntitiesV2.PurchaseOrder purchaseOrder;
			var paymentPickerViewModel = new PaymentPickerViewModel();

			try
			{
				purchaseOrder = TransactionLibrary.GetBasket();
			}
			catch (Exception ex)
			{
				Log.Write(ex, ConfigurationPolicy.ErrorLog);
				return null;
			}

			if (!purchaseOrder.OrderLines.Any())
			{
				return null;
			}

			var shippingAddress =
				 TransactionLibrary.GetShippingInformation(Ucommerce.Constants.DefaultShipmentAddressName);

			var shippingCountry = shippingAddress.Country;

			if (shippingCountry != null)
			{
				paymentPickerViewModel.ShippingCountry = shippingCountry.Name;

				var availablePaymentMethods = TransactionLibrary.GetPaymentMethods(shippingCountry);

				var existingPayment = purchaseOrder.Payments.FirstOrDefault();
				paymentPickerViewModel.SelectedPaymentMethodId = existingPayment != null
					? existingPayment.PaymentMethod.PaymentMethodId
					: -1;
				var priceGroup = PriceGroupRepository.SingleOrDefault(pg => pg.Guid == CatalogContext.CurrentPriceGroup.Guid);

				foreach (var availablePaymentMethod in availablePaymentMethods)
				{
					var option = new SelectListItem();
					decimal feePercent = availablePaymentMethod.FeePercent;
					var localizedPaymentMethod = availablePaymentMethod.PaymentMethodDescriptions.FirstOrDefault(s =>
						s.CultureCode.Equals(CultureInfo.CurrentCulture.ToString()));
					var fee = availablePaymentMethod.GetFeeForPriceGroup(priceGroup);
					var formattedFee = new Money(fee == null ? 0 : fee.Fee, purchaseOrder.BillingCurrency.ISOCode);

					if (localizedPaymentMethod != null)
					{
						var displayName = localizedPaymentMethod.DisplayName;
						if (string.IsNullOrWhiteSpace(displayName)) displayName = availablePaymentMethod.Name;

						option.Text = String.Format(" {0} ({1} + {2}%)", displayName, formattedFee, feePercent.ToString("0.00"));
						option.Value = availablePaymentMethod.PaymentMethodId.ToString();
						option.Selected = availablePaymentMethod.PaymentMethodId == paymentPickerViewModel.SelectedPaymentMethodId;
					}

					paymentPickerViewModel.AvailablePaymentMethods.Add(option);
				}
			}

			TransactionLibrary.ExecuteBasketPipeline();

			paymentPickerViewModel.NextStepUrl = GetNextStepUrl(nextStepId);
			paymentPickerViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

			InsightUcommerce.SendInteraction(purchaseOrder, "Checkout", "View Payment Options");

			return paymentPickerViewModel;
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

			message = null;
			return true;
		}

		public virtual void CreatePayment(PaymentPickerViewModel createPaymentViewModel)
		{
			TransactionLibrary.CreatePayment(createPaymentViewModel.SelectedPaymentMethodId, -1m, false, true);
			TransactionLibrary.ExecuteBasketPipeline();

			InsightUcommerce.SendBasketInteraction("Checkout", $"Payment Method Selected {createPaymentViewModel.SelectedPaymentMethodId}");
		}

		private string GetNextStepUrl(Guid nextStepId)
		{
			var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

			return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
		}

		private string GetPreviousStepUrl(Guid previousStepId)
		{
			var previousStepUrl = Pages.UrlResolver.GetPageNodeUrl(previousStepId);

			return Pages.UrlResolver.GetAbsoluteUrl(previousStepUrl);
		}
	}
}