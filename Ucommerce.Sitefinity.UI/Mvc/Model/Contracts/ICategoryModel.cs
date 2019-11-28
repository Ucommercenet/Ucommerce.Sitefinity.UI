using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Category MVC widget.
    /// </summary>
    public interface ICategoryModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        CategoryNavigationViewModel CreateViewModel();
    }
}
