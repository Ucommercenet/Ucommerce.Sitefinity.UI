using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface IMiniBasketModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        MiniBasketRenderingViewModel CreateViewModel(string refreshUrl);
    }
}
