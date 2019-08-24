using System.Collections.Generic;
using Ucommerce.Sitefinity.UI.Mvc.ViewModels;

namespace Ucommerce.Sitefinity.UI.Mvc.Model
{
    public interface IFacetsFilterModel
    {
        IList<FacetViewModel> CreateViewModel(string categoryName);
    }
}
