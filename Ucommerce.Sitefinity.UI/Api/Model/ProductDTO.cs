using System;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for storing product information.
    /// </summary>
    public class ProductDTO
    {
        public Guid ProductGuid { get; set; }
        public Guid VariantGuid { get; set; }
    }
}
