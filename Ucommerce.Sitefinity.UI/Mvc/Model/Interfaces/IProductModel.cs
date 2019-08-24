using System;
using UCommerce.EntitiesV2;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public interface IProductModel
    {
        ProductListViewModel CreateListViewModel();

        ProductDetailViewModel CreateDetailsViewModel();

        string GetProductUrl(Category category, Product product, bool openInSamePage, Guid detailPageId);
    }
}
