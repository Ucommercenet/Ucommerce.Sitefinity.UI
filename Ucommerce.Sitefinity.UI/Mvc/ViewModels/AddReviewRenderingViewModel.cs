using System;

namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class AddReviewRenderingViewModel
    {
        public Guid CategoryGuid { get; set; }
        public Guid ProductGuid { get; set; }
        public string SubmitReviewUrl { get; set; }
    }
}
