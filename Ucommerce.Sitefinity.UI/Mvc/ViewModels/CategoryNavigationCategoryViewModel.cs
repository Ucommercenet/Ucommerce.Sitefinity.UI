using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class CategoryNavigationCategoryViewModel
    {
        public string DisplayName { get; set; }

        public string Url { get; set; }

        public bool IsActive { get; set; }

        public IList<CategoryNavigationCategoryViewModel> Categories { get; set; }
    }
}
