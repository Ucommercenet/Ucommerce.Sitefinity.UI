using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Shipping Picker MVC widget.
    /// </summary>
    public interface IShippingPickerModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ShippingPickerViewModel GetViewModel();
        void CreateShipment(ShippingPickerViewModel createShipmentViewModel);
    }
}
