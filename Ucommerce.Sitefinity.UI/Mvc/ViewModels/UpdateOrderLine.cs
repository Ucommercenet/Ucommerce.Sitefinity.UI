using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// DTO class used for updating an item in a order.
    /// </summary>
    public class UpdateOrderLine
    {
        public int OrderLineId { get; set; }
        public int OrderLineQty { get; set; }
    }
}
