using System;
using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used to list the information visualized by the Category Navigation.
    /// </summary>
    public class CategoryNavigationViewModel
    {
        public bool AllowChangingCurrency { get; set; }
        public string BaseUrl { get; set; }
        public IList<CategoryNavigationCategoryViewModel> Categories { get; set; }
        public IList<CategoryNavigationCurrencyViewModel> Currencies { get; set; }
        public CategoryNavigationCurrencyViewModel CurrentCurrency { get; set; }
        public bool HideMiniBasket { get; set; }
        public string ImageUrl { get; set; }
        public string Localizations { get; set; }
        public Guid ProductDetailsPageId { get; set; }
        public Dictionary<string, string> Routes { get; set; }
        public string SearchPageUrl { get; set; }

        public CategoryNavigationViewModel()
        {
            Routes = new Dictionary<string, string>();
        }
    }
}
