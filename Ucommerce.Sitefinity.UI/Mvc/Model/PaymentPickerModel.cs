using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Services;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using UCommerce.Sitefinity.UI.Mvc.Services;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Sitefinity.UI.Pages;
using ObjectFactory = Ucommerce.Infrastructure.ObjectFactory;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Payment Picker MVC widget.
    /// </summary>
    public class PaymentPickerModel : IPaymentPickerModel
    {
        public IRepository<PriceGroup> PriceGroupRepository = ObjectFactory.Instance.Resolve<IRepository<PriceGroup>>();
        private readonly Guid nextStepId;
        private readonly Guid previousStepId;
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IInsightUcommerceService InsightUcommerce => UCommerceUIModule.Container.Resolve<IInsightUcommerceService>();
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        public PaymentPickerModel(Guid? nextStepId = null, Guid? previousStepId = null)
        {
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.previousStepId = previousStepId ?? Guid.Empty;
        }

        public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            object mode = null;

            if (parameters.TryGetValue("mode", out mode) && mode != null)
            {
                if (mode.ToString() == "index")
                {
                    if (SystemManager.IsDesignMode)
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

            var paymentMethod = PaymentMethod.Get(createPaymentViewModel.SelectedPaymentMethodId);
            InsightUcommerce.SendInteraction("Checkout > Select payment method", paymentMethod.Name);
        }

        public virtual PaymentPickerViewModel GetViewModel()
        {
            PurchaseOrder purchaseOrder;
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
                    var feePercent = availablePaymentMethod.FeePercent;
                    var localizedPaymentMethod = availablePaymentMethod.PaymentMethodDescriptions.FirstOrDefault(s =>
                        s.CultureCode.Equals(CultureInfo.CurrentCulture.ToString()));
                    var fee = availablePaymentMethod.GetFeeForPriceGroup(priceGroup);
                    var formattedFee = new Money(fee == null ? 0 : fee.Fee, purchaseOrder.BillingCurrency.ISOCode);

                    if (localizedPaymentMethod != null)
                    {
                        var displayName = localizedPaymentMethod.DisplayName;
                        if (string.IsNullOrWhiteSpace(displayName))
                        {
                            displayName = availablePaymentMethod.Name;
                        }

                        option.Text = string.Format(" {0} ({1} + {2}%)", displayName, formattedFee, feePercent.ToString("0.00"));
                        option.Value = availablePaymentMethod.PaymentMethodId.ToString();
                        option.Selected = availablePaymentMethod.PaymentMethodId == paymentPickerViewModel.SelectedPaymentMethodId;
                    }

                    paymentPickerViewModel.AvailablePaymentMethods.Add(option);
                }
            }

            TransactionLibrary.ExecuteBasketPipeline();

            paymentPickerViewModel.NextStepUrl = GetNextStepUrl(nextStepId);
            paymentPickerViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

            return paymentPickerViewModel;
        }

        private string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = UrlResolver.GetPageNodeUrl(nextStepId);

            return UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }

        private string GetPreviousStepUrl(Guid previousStepId)
        {
            var previousStepUrl = UrlResolver.GetPageNodeUrl(previousStepId);

            return UrlResolver.GetAbsoluteUrl(previousStepUrl);
        }
    }
}
