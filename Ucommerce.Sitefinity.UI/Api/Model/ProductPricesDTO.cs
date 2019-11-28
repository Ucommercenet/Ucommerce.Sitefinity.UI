using System;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for storing product pricing information.
    /// </summary>
    public class ProductPricesDTO
    {
        public Guid ProductGuid { get; set; }

        public string Price { get; set; }

        public string ListPrice { get; set; }
    }
}
