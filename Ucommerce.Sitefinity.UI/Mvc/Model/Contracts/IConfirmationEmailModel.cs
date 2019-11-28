using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Confirmation Email MVC widget.
    /// </summary>
    public interface IConfirmationEmailModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ConfirmationEmailViewModel GetViewModel(string orderGuid);
    }
}
