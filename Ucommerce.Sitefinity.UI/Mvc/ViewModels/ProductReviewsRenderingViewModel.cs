using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ProductReviewsRenderingViewModel
    {
        public string Ip { get; set; }
        public List<ProductReview> Reviews { get; set; }

        public ProductReviewsRenderingViewModel()
        {
            Reviews = new List<ProductReview>();
        }
    }
}
