using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Confirmation Message MVC widget.
    /// </summary>
    public interface IConfirmationMessageModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ConfirmationMessageViewModel GetViewModel(string headline, string message, string orderGuid);
    }
}
