using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the information associated with list of products.
    /// </summary>
    public class ProductListViewModel
    {
        public string CssClass { get; set; }
        public int CurrentPage { get; set; }
        public string PagingUrlTemplate { get; set; }
        public IList<ProductDTO> Products { get; set; }
        public Dictionary<string, string> Routes { get; set; }
        public bool ShowPager { get; set; }
        public IList<SortOption> Sorting { get; set; }
        public int TotalCount { get; set; }
        public int? TotalPagesCount { get; set; }

        public ProductListViewModel()
        {
            Routes = new Dictionary<string, string>();
            Sorting = new List<SortOption>();
        }
    }
}
