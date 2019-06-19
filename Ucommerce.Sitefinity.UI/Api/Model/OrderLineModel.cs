using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ucommerce.Sitefinity.UI.Api.Model
{
    public class OrderLineModel
    {
        public string ProductUrl { get; set; }

        public string ThumbnailImageMediaUrl { get; set; }

        public string ProductName { get; set; }

        public string UnitPrice { get; set; }

        public string Total { get; set; }

        public int Quantity { get; set; }

        public int OrderlineId { get; set; }

        public bool HasDiscount { get; set; }
    }
}
