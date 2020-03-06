namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class AddReviewDTO
    {
        public int Rating { get; set; }
        public string ReviewHeadline { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedOnForMeta { get; set; }
    }
}