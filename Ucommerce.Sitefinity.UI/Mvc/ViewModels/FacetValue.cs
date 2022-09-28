namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// DTO class used for storing the details of a facet.
    /// </summary>
    public class FacetValue
    {
        public int FacetValueHits { get; set; }
        public string FacetValueName { get; set; }

        public FacetValue(string name, int hits)
        {
            FacetValueName = name;
            FacetValueHits = hits;
        }
    }
}
