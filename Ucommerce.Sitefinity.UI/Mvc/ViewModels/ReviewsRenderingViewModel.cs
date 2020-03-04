using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ReviewsRenderingViewModel
    {
        public List<Review> Reviews { get; set; }

        public ReviewsRenderingViewModel()
        {
            Reviews = new List<Review>();
        }
    }
}