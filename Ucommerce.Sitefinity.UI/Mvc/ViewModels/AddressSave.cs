using System.ComponentModel.DataAnnotations;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// DTO class used to persist information related to single address.
    /// </summary>
    public class AddressSave
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number")]
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
