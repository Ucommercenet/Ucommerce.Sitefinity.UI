using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class VariantTypeViewModel
    {
        public VariantTypeViewModel()
        {
            this.Values = new List<VariantViewModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<VariantViewModel> Values { get; set; }
    }
}
