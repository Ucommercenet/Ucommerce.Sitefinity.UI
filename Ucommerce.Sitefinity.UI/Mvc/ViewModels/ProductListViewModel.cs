using System.Collections.Generic;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ProductListViewModel
    {
        public ProductListViewModel()
        {
            this.Routes = new Dictionary<string, string>();
        }

        public string CssClass { get; set; }

        public IList<ProductViewModel> Products { get; set; }

        public int? TotalPagesCount { get; set; }

        public int CurrentPage { get; set; }

        public bool ShowPager { get; set; }

        public int TotalCount { get; set; }

        public string PagingUrlTemplate { get; set; }

        public Dictionary<string, string> Routes { get; set; }
    }
}
