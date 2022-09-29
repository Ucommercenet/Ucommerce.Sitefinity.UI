using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Mini Basket MVC widget.
    /// </summary>
    public interface IMiniBasketModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        MiniBasketRenderingViewModel CreateViewModel(string refreshUrl);
        MiniBasketRefreshViewModel Refresh();
    }
}
