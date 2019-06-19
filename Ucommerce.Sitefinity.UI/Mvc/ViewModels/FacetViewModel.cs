using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
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
