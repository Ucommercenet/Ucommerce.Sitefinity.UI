using System.Collections.Generic;
using System.Web.Mvc;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Address MVC widget.
    /// </summary>
    public interface IAddressModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        AddressRenderingViewModel GetViewModel();
        JsonResult Save(AddressSaveViewModel addressRendering);
        Dictionary<string, string> ErrorMessage(ModelStateDictionary status);
    }
}
