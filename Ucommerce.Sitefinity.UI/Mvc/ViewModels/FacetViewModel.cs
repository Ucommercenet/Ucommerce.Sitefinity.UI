using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the information associated with facets.
    /// </summary>
    public class FacetViewModel
    {
        public string DisplayName { get; set; }
        public IList<FacetValue> FacetValues { get; set; }
        public string Name { get; set; }

        public FacetViewModel()
        {
            FacetValues = new List<FacetValue>();
        }
    }
}
