using System.Collections.Generic;
using System.Web.Mvc;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
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

        public class Address
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public string PhoneNumber { get; set; }
            public string MobilePhoneNumber { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Attention { get; set; }
            public string CompanyName { get; set; }
            public int CountryId { get; set; }
        }
    }
}
