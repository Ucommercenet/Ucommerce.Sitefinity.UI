using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class AddressSaveViewModel
    {
        public AddressSaveViewModel()
        {
            ShippingAddress = new Address();
            BillingAddress = new Address();
            IsShippingAddressDifferent = false;
        }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public bool IsShippingAddressDifferent { get; set; }



        public class Address
        {
            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string EmailAddress { get; set; }

            [PhoneAttribute(ErrorMessage = "Invalid Phone Number")]
            public string PhoneNumber { get; set; }
            public string MobilePhoneNumber { get; set; }

            [Required(ErrorMessage = "Address line 1 is required")]
            public string Line1 { get; set; }
            public string Line2 { get; set; }

            [Required(ErrorMessage = "Postal code is required")]
            public string PostalCode { get; set; }
            [Required(ErrorMessage = "City is required")]
            public string City { get; set; }
            public string State { get; set; }
            public string Attention { get; set; }
            public string CompanyName { get; set; }
            public int CountryId { get; set; }
        }
    }
}
