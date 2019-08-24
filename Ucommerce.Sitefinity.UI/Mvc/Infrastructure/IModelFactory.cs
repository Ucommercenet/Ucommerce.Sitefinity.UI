using System;
using Ucommerce.Sitefinity.UI.Mvc.Model;

namespace Ucommerce.Sitefinity.UI.Mvc.Infrastructure
{    
    public interface IModelFactory
    {
        IFacetsFilterModel CreateFacetsFilterModel();

        IProductModel CreateProductModel(int itemsPerPage, bool openInSamePage, bool isManualSelectionMode, Guid? detailsPageId, string productIds, string categoryIds);

        ICategoryModel CreateCategoryModel(bool hideMiniBasket, bool allowChangingCurrency, Guid? imageId, Guid? categoryPageId, Guid? searchPageId);
    }
}
