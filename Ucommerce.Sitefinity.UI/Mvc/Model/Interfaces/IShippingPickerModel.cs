using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IShippingPickerModel
    {
        ShippingPickerViewModel GetViewModel();
        void CreateShipment(ShippingPickerViewModel createShipmentViewModel);
    }
}
