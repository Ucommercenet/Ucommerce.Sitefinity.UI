using System.Collections.Generic;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IShippingPickerModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ShippingPickerViewModel GetViewModel();
        void CreateShipment(ShippingPickerViewModel createShipmentViewModel);
    }
}
