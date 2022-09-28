using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Cart MVC widget.
    /// </summary>
    public interface ICartModel
    {
        CartUpdateBasketViewModel AddVoucher(CartUpdateBasket model);
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl);
        CartUpdateBasketViewModel RemoveVoucher(CartUpdateBasket model);
        CartUpdateBasketViewModel Update(CartUpdateBasket model);
    }
}
