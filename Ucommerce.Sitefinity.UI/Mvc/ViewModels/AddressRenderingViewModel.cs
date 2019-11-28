using System.Collections.Generic;
using System.Web.Mvc;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used to list the information related to the addresses associated with an order.
    /// </summary>
    public class AddressRenderingViewModel
    {
        public AddressRenderingViewModel()
        {
            ShippingAddress = new Address();
            BillingAddress = new Address();
            AvailableCountries = new List<SelectListItem>();
            IsShippingAddressDifferent = false;
        }

        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public bool IsShippingAddressDifferent { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public string SaveAddressUrl { get; set; }
        public string NextStepUrl { get; set; }
        public string PreviousStepUrl { get; set; }
    }
}

