using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the information associated with facets.
    /// </summary>
    public class FacetViewModel
    {
        public FacetViewModel()
        {
            FacetValues = new List<FacetValue>();
        }

        public IList<FacetValue> FacetValues { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}
