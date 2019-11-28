using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    interface ICartModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl);
        CartUpdateBasketViewModel Update(CartUpdateBasket model);
    }
}
