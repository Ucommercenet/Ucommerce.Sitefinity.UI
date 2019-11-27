using System.Collections.Generic;
using System.Web.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IAddressModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        AddressRenderingViewModel GetViewModel();
        JsonResult Save(AddressSaveViewModel addressRendering);
    }
}
