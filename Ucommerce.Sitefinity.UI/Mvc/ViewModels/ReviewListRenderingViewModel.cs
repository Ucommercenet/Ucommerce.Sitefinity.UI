using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ReviewListRenderingViewModel
    {
        public List<Review> Reviews { get; set; }

        public ReviewListRenderingViewModel()
        {
            Reviews = new List<Review>();
        }
    }
}