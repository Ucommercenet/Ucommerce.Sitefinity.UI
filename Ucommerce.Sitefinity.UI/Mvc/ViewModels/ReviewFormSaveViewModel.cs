namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    public class ReviewFormSaveViewModel
    {
        public string ProductGuid { get; set; }
        public string CategoryGuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
    }
}