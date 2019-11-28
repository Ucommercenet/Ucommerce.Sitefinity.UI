using System.Collections.Generic;
using System.Web.Mvc;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing payment provider information.
    /// </summary>
    public class PaymentPickerViewModel
    {
        public PaymentPickerViewModel()
        {
            AvailablePaymentMethods = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailablePaymentMethods { get; set; }
        public int SelectedPaymentMethodId { get; set; }
        public string ShippingCountry { get; set; }
        public string NextStepUrl { get; set; }
        public string PreviousStepUrl { get; set; }
    }
}
