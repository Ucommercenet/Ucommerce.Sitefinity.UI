using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ProductReviewsRenderingViewModel
    {
        public ProductReviewsRenderingViewModel()
        {
            Reviews = new List<ProductReview>();
        }

        public List<ProductReview> Reviews { get; set; }
        public string Ip { get; set; }
    }
}