using System;
using System.Collections.Generic;

namespace Ucommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class CategoryNavigationViewModel
    {
        public CategoryNavigationViewModel()
        {
            this.Routes = new Dictionary<string, string>();
        }

        public IList<CategoryNavigationCategoryViewModel> Categories { get; set; }

        public string ImageUrl { get; set; }

        public bool HideMiniBasket { get; set; }

        public string SearchPageUrl { get; set; }

        public bool AllowChangingCurrency { get; set; }

        public IList<CategoryNavigationCurrencyViewModel> Currencies { get; set; }

        public CategoryNavigationCurrencyViewModel CurrentCurrency { get; set; }

        public Dictionary<string, string> Routes { get; set; }

        public Guid PageId { get; set; }

        public int CategoryId { get; set; }
    }
}
