using System.Collections.Generic;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IConfirmationMessageModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ConfirmationMessageViewModel GetViewModel(string headline, string message);
    }
}
