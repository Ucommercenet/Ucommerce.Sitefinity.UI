using System;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for storing product review information.
    /// </summary>
    public class ProductReviewDTO
    {
        public string Comments { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Sku { get; set; }
        public DateTime Submitted { get; set; }
        public string Title { get; set; }
    }
}
