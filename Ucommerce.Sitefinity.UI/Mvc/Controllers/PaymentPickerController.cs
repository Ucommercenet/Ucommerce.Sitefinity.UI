using System;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uPaymentPicker_MVC", Title = "Payment Picker", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class PaymentPickerController : Controller
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;

        public PaymentPickerController()
        {
            _transactionLibraryInternal = ObjectFactory.Instance.Resolve<TransactionLibraryInternal>();
        }

        public ActionResult Index()
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

            foreach (var availablePaymentMethod in availablePaymentMethods)
            {
                var option = new SelectListItem();
                decimal feePercent = availablePaymentMethod.FeePercent;
                var fee = availablePaymentMethod.GetFeeForCurrency(basket.BillingCurrency);
                var formattedFee = new Money((fee == null ? 0 : fee.Fee), basket.BillingCurrency);

                option.Text = String.Format(" {0} ({1} + {2}%)", availablePaymentMethod.Name, formattedFee,
                    feePercent.ToString("0.00"));
                option.Value = availablePaymentMethod.PaymentMethodId.ToString();
                option.Selected = availablePaymentMethod.PaymentMethodId == paymentPickerViewModel.SelectedPaymentMethodId;

                paymentPickerViewModel.AvailablePaymentMethods.Add(option);
            }

            return View(paymentPickerViewModel);
        }

        [HttpPost]
        public ActionResult CreatePayment(PaymentPickerViewModel createPaymentViewModel)
        {
            _transactionLibraryInternal.CreatePayment(createPaymentViewModel.SelectedPaymentMethodId, -1m, false, true);
            _transactionLibraryInternal.ExecuteBasketPipeline();

            return Redirect("/preview");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }
    }
}
