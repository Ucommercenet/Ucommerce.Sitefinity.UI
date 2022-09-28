using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class VariantTypeViewModel
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<VariantViewModel> Values { get; set; }

        public VariantTypeViewModel()
        {
            Values = new List<VariantViewModel>();
        }
    }
}
