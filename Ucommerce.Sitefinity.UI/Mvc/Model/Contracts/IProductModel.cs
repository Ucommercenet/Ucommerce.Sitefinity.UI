using System;
using Ucommerce.EntitiesV2;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Product MVC widget.
    /// </summary>
    public interface IProductModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);

        ProductListViewModel CreateListViewModel();

        ProductDetailViewModel CreateDetailsViewModel();

        string GetProductUrl(Category category, Product product, bool openInSamePage, Guid detailPageId);
    }
}
