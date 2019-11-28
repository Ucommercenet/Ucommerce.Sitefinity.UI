using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Interfaces
{
    public interface IMiniBasketService
    {
        MiniBasketRefreshViewModel Refresh();
    }
}
