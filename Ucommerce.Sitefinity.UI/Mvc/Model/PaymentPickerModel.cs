using System;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Infrastructure;
using UCommerce.Runtime;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class PaymentPickerModel : IPaymentPickerModel
    {
        private Guid nextStepId;
        private Guid previousStepId;
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public PaymentPickerModel(Guid? nextStepId = null, Guid? previousStepId = null)
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
            this.nextStepId = nextStepId ?? Guid.Empty;
            this.previousStepId = previousStepId ?? Guid.Empty;
        }

        public PaymentPickerViewModel GetViewModel()
        {
            var paymentPickerViewModel = new PaymentPickerViewModel();

            var basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
            var shippingCountry = UCommerce.Api.TransactionLibrary.GetCountries().SingleOrDefault(x => x.Name == "Germany");

            paymentPickerViewModel.ShippingCountry = shippingCountry.Name;

            var availablePaymentMethods = _transactionLibraryInternal.GetPaymentMethods(shippingCountry);

            var existingPayment = basket.Payments.FirstOrDefault();
            paymentPickerViewModel.SelectedPaymentMethodId = existingPayment != null
                ? existingPayment.PaymentMethod.PaymentMethodId
                : -1;
            var priceGroup = SiteContext.Current.CatalogContext.CurrentPriceGroup;

            foreach (var availablePaymentMethod in availablePaymentMethods)
            {
                var option = new SelectListItem();
                decimal feePercent = availablePaymentMethod.FeePercent;
                var fee = availablePaymentMethod.GetFeeForPriceGroup(priceGroup);
                var formattedFee = new Money(fee == null ? 0 : fee.Fee, basket.BillingCurrency);

                option.Text = String.Format(" {0} ({1} + {2}%)", availablePaymentMethod.Name, formattedFee,
                    feePercent.ToString("0.00"));
                option.Value = availablePaymentMethod.PaymentMethodId.ToString();
                option.Selected = availablePaymentMethod.PaymentMethodId == paymentPickerViewModel.SelectedPaymentMethodId;

                paymentPickerViewModel.AvailablePaymentMethods.Add(option);
            }

            paymentPickerViewModel.NextStepUrl = GetNextStepUrl(nextStepId);
            paymentPickerViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

            return paymentPickerViewModel;
        }

        public void CreatePayment(PaymentPickerViewModel createPaymentViewModel)
        {
            _transactionLibraryInternal.CreatePayment(createPaymentViewModel.SelectedPaymentMethodId, -1m, false, true);
            _transactionLibraryInternal.ExecuteBasketPipeline();
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
