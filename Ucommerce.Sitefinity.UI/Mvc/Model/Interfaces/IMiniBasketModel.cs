using System.Collections.Generic;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface IMiniBasketModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        MiniBasketRenderingViewModel CreateViewModel(string refreshUrl);
    }
}
