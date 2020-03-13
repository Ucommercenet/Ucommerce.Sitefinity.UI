using System;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ProductReview
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public int? Rating { get; set; }
        public string Ip { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ProductGuid { get; set; }
        public Guid CategoryGuid { get; set; }
    }
}