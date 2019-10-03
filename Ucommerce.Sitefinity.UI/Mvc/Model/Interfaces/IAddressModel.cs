using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface IAddressModel
    {
        AddressRenderingViewModel GetViewMode(string saveUrl);
        void EditShippingInformation(AddressSave shippingAddress);
        void EditBillingInformation(AddressSave billingAddress);
    }
}
