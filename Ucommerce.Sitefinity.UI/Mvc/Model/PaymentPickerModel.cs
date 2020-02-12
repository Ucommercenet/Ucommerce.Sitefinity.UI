﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Transactions;
using ObjectFactory = UCommerce.Infrastructure.ObjectFactory;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The Model class of the Payment Picker MVC widget.
    /// </summary>
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

        public virtual PaymentPickerViewModel GetViewModel()
        {
            PurchaseOrder purchaseOrder;
            var paymentPickerViewModel = new PaymentPickerViewModel();

            try
            {
                purchaseOrder = _transactionLibraryInternal.GetBasket().PurchaseOrder;
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
            var shippingCountry = TransactionLibrary.GetCountries().SingleOrDefault(x => x.Name == "Germany");

            if (shippingCountry != null)
            {
                paymentPickerViewModel.ShippingCountry = shippingCountry.Name;

                var availablePaymentMethods = _transactionLibraryInternal.GetPaymentMethods(shippingCountry);

                var existingPayment = purchaseOrder.Payments.FirstOrDefault();
                paymentPickerViewModel.SelectedPaymentMethodId = existingPayment != null
                    ? existingPayment.PaymentMethod.PaymentMethodId
                    : -1;
                var priceGroup = SiteContext.Current.CatalogContext.CurrentPriceGroup;

                foreach (var availablePaymentMethod in availablePaymentMethods)
                {
                    var option = new SelectListItem();
                    decimal feePercent = availablePaymentMethod.FeePercent;
                    var fee = availablePaymentMethod.GetFeeForPriceGroup(priceGroup);
                    var formattedFee = new Money(fee == null ? 0 : fee.Fee, purchaseOrder.BillingCurrency);

                    option.Text = String.Format(" {0} ({1} + {2}%)", availablePaymentMethod.Name, formattedFee,
                        feePercent.ToString("0.00"));
                    option.Value = availablePaymentMethod.PaymentMethodId.ToString();
                    option.Selected = availablePaymentMethod.PaymentMethodId ==
                                      paymentPickerViewModel.SelectedPaymentMethodId;

                    paymentPickerViewModel.AvailablePaymentMethods.Add(option);
                }
            }

            _transactionLibraryInternal.ExecuteBasketPipeline();

            paymentPickerViewModel.NextStepUrl = GetNextStepUrl(nextStepId);
            paymentPickerViewModel.PreviousStepUrl = GetPreviousStepUrl(previousStepId);

            return paymentPickerViewModel;
        }

        public virtual bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            message = "No order is available";
            return true;
        }

        public virtual void CreatePayment(PaymentPickerViewModel createPaymentViewModel)
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