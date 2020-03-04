using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ReviewsRenderingViewModel
    {
        public ReviewsRenderingViewModel()
        {
            Reviews = new List<Review>();
        }

        public List<Review> Reviews { get; set; }
        public string Ip { get; set; }
    }
}