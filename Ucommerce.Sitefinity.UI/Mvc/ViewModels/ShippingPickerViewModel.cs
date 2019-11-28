using System.Collections.Generic;
using System.Web.Mvc;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing shipping provider information.
    /// </summary>
    public class ShippingPickerViewModel
    {
        public ShippingPickerViewModel()
        {
            AvailableShippingMethods = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailableShippingMethods { get; set; }
        public int SelectedShippingMethodId { get; set; }
        public string ShippingCountry { get; set; }
        public string NextStepUrl { get; set; }
        public string PreviousStepUrl { get; set; }
    }
}
