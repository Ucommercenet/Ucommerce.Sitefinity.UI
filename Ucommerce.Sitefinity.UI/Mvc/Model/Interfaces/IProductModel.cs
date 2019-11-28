using System;
using UCommerce.EntitiesV2;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public interface IProductModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);

        ProductListViewModel CreateListViewModel();

        ProductDetailViewModel CreateDetailsViewModel();

        string GetProductUrl(Category category, Product product, bool openInSamePage, Guid detailPageId);
    }
}
