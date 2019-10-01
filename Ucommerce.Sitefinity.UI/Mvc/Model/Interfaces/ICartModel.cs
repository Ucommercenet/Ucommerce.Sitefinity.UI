using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface ICartModel
    {
        CartRenderingViewModel CreateViewModel(string refreshUrl, string removeOrderLineUrl);
        CartUpdateBasketViewModel UpdateViewModel(CartUpdateBasket model);
    }
}
