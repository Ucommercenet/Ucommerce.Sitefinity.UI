using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Cart MVC widget.
    /// </summary>
    public interface ICartModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        CartRenderingViewModel GetViewModel(string refreshUrl, string removeOrderLineUrl);
        CartUpdateBasketViewModel Update(CartUpdateBasket model);
        CartUpdateBasketViewModel RemoveVoucher(CartUpdateBasket model);
        CartUpdateBasketViewModel AddVoucher(CartUpdateBasket model);
    }
}
