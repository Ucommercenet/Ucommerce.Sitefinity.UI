using System.Web.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IAddressModel
    {
        AddressRenderingViewModel GetViewModel();
        JsonResult Save(AddressSaveViewModel addressRendering);
    }
}
