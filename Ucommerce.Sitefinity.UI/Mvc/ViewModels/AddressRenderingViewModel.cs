﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used to list the information related to the addresses associated with an order.
    /// </summary>
    public class AddressRenderingViewModel
    {
        public IList<SelectListItem> AvailableCountries { get; set; }
        public AddressViewModel BillingAddress { get; set; }
        public bool IsShippingAddressDifferent { get; set; }
        public string NextStepUrl { get; set; }
        public string PreviousStepUrl { get; set; }
        public string SaveAddressUrl { get; set; }
        public AddressViewModel ShippingAddress { get; set; }

        [Obsolete("Use BillingAddress instead")]
        public AddressViewModel BillingAddressDTO
        {
            get => BillingAddress;
            set => BillingAddress = value;
        }

        [Obsolete("Use ShippingAddress instead")]
        public AddressViewModel ShippingAddressDTO
        {
            get => ShippingAddress;
            set => ShippingAddress = value;
        }

        public AddressRenderingViewModel()
        {
            ShippingAddress = new AddressViewModel();
            BillingAddress = new AddressViewModel();
            AvailableCountries = new List<SelectListItem>();
            IsShippingAddressDifferent = false;
        }
    }
}
