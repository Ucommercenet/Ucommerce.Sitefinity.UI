using System;
using System.Collections.Generic;
using Ucommerce.Search.Models;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Product MVC widget.
    /// </summary>
    public interface IProductModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ProductDetailViewModel CreateDetailsViewModel();
        ProductListViewModel CreateListViewModel();
        string GetProductUrl(Category category, Product product, bool openInSamePage, Guid detailPageId);
    }
}
