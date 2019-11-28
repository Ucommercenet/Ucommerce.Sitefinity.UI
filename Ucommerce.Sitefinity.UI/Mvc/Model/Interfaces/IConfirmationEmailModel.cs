using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IConfirmationEmailModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ConfirmationEmailViewModel GetViewModel(string orderGuid);
    }
}
