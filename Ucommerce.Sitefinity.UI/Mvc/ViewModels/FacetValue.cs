using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// DTO class used for storing the details of a facet.
    /// </summary>
    public class FacetValue
    {
        public FacetValue(string name, int hits)
        {
            FacetValueName = name;
            FacetValueHits = hits;
        }

        public string FacetValueName { get; set; }

        public int FacetValueHits { get; set; }
    }
}
