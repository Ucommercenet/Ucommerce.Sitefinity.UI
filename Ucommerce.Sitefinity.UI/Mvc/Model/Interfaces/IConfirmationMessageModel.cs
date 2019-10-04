using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IConfirmationMessageModel
    {
        ConfirmationMessageViewModel GetViewModel(string headline, string message);
    }
}
