using System;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ReviewFormRenderingViewModel
    {
        public string SubmitReviewUrl { get; set; }
        public Guid ProductGuid { get; set; }
        public Guid CategoryGuid { get; set; }
    }
}