using System;
using Ucommerce.Sitefinity.UI.Mvc.Controllers;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public class SpaCheckoutModel : ISpaCheckoutModel
    {
        private Guid nextStepId;

        public SpaCheckoutModel(Guid? nextStepId = null)
        {
            this.nextStepId = nextStepId ?? Guid.Empty;
        }

        public SpaCheckoutViewModel GetViewModel(AddressSaveViewModel addressRendering)
        {
            var spaCkeckoutViewModel = new SpaCheckoutViewModel();

            var addressCont = new AddressController();
            var shipCont = new ShippingPickerController();
            var paymentCont = new PaymentPickerController();

            addressCont.Save(addressRendering);
            shipCont.CreateShipment();
            paymentCont.CreatePayment();

            spaCkeckoutViewModel.NextStepUrl = GetNextStepUrl(nextStepId);

            return spaCkeckoutViewModel;
        }

        public string GetNextStepUrl(Guid nextStepId)
        {
            var nextStepUrl = Pages.UrlResolver.GetPageNodeUrl(nextStepId);

            return Pages.UrlResolver.GetAbsoluteUrl(nextStepUrl);
        }
    }
}
