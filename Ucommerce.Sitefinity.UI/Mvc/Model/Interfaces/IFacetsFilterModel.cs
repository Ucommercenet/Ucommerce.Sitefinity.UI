using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    public interface IFacetsFilterModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        IList<FacetViewModel> CreateViewModel();
    }
}
