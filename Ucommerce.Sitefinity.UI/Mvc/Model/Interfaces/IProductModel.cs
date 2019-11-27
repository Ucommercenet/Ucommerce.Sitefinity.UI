using System;
using UCommerce.EntitiesV2;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;
using System.Collections.Generic;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public interface IProductModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);

        ProductListViewModel CreateListViewModel();

        ProductDetailViewModel CreateDetailsViewModel();

        string GetProductUrl(Category category, Product product, bool openInSamePage, Guid detailPageId);
    }
}
