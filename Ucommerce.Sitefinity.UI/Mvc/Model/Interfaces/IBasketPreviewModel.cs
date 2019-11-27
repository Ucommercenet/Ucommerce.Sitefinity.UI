using System.Collections.Generic;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.EntitiesV2;

namespace Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IBasketPreviewModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        BasketPreviewViewModel GetViewModel();
        string GetPaymentUrl();
    }
}
