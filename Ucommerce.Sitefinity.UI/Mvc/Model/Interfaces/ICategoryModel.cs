using System.Collections.Generic;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public interface ICategoryModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        CategoryNavigationViewModel CreateViewModel();
    }
}
