using System;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    public class ProductPricesViewModel
    {
        public Guid ProductGuid { get; set; }

        public string Price { get; set; }

        public string ListPrice { get; set; }
    }
}
