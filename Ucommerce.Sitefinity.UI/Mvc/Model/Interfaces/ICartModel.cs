using System.Collections.Generic;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface ICartModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl);
        CartUpdateBasketViewModel Update(CartUpdateBasket model);
    }
}
