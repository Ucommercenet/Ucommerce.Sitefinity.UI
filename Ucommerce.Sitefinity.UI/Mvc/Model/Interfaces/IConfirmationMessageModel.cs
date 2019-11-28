using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IConfirmationMessageModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ConfirmationMessageViewModel GetViewModel(string headline, string message);
    }
}
