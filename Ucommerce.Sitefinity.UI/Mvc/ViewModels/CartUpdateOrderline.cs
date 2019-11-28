using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class CartUpdateOrderline
    {
        public int OrderlineId { get; set; }
        public int Quantity { get; set; }
        public string Total { get; set; }
        public decimal Discount { get; set; }
        public string Tax { get; set; }
        public string Price { get; set; }
        public string PriceWithDiscount { get; set; }
    }
}
