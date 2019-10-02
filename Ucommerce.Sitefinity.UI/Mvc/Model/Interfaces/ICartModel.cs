using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface ICartModel
    {
        CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl);
        CartUpdateBasketViewModel Update(CartUpdateBasket model);
    }
}
