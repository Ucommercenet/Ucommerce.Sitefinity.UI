using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface IMiniBasketModel
    {
        MiniBasketRenderingViewModel CreateViewModel(string refreshUrl);
    }
}
