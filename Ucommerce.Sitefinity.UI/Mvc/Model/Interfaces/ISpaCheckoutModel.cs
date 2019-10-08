using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface ISpaCheckoutModel
    {
        SpaCheckoutViewModel GetViewModel(AddressSaveViewModel addressRendering);
    }
}
