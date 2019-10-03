using System.Web.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface IAddressModel
    {
        AddressRenderingViewModel GetViewMode(string saveUrl);
        JsonResult Save(AddressSaveViewModel addressRendering, System.Web.Mvc.ModelStateDictionary modelState);
    }
}
