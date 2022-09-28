using System;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ProductReview
    {
        public Guid CategoryGuid { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Email { get; set; }
        public string Ip { get; set; }
        public string Name { get; set; }
        public Guid ProductGuid { get; set; }
        public int? Rating { get; set; }
        public string Title { get; set; }
    }
}
