namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class AddReviewSaveViewModel
    {
        public string ProductGuid { get; set; }
        public string CategoryGuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string ReviewHeadline { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedOnForMeta { get; set; }
    }
}