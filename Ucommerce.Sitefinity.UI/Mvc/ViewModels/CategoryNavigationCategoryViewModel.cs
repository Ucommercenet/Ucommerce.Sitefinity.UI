using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used to vizualize a single category item listed by the Category Navigation.
    /// </summary>
    public class CategoryNavigationCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string DisplayName { get; set; }

        public string Url { get; set; }

        public bool IsActive { get; set; }

        public IList<CategoryNavigationCategoryViewModel> Categories { get; set; }
    }
}
