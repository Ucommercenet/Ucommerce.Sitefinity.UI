using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucommerce.Sitefinity.UI.Api.Model
{
    public class ProductReviewModel
    {
        public string Sku { get; set; }

        public int Rating { get; set; }

        public string Title { get; set; }

        public string Email { get; set; }

        public DateTime Submitted { get; set; }

        public string Comments { get; set; }

        public string Name { get; set; }
    }
}
