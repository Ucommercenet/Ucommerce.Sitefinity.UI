using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class PaymentPickerViewModel
    {
        public PaymentPickerViewModel()
        {
            AvailablePaymentMethods = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailablePaymentMethods { get; set; }
        public int SelectedPaymentMethodId { get; set; }
        public string ShippingCountry { get; set; }
    }
}
