using System.Collections.Generic;
using System.Web.Mvc;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IAddressModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        AddressRenderingViewModel GetViewModel();
        JsonResult Save(AddressSaveViewModel addressRendering);
    }
}
