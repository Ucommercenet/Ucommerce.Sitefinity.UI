using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Basket Preview MVC widget.
    /// </summary>
    public interface IBasketPreviewModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        BasketPreviewViewModel GetViewModel();
        string GetPaymentUrl();
    }
}
