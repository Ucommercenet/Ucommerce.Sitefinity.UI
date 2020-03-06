namespace UCommerce.Sitefinity.UI.Mvc.ViewModels
{
    /// <summary>
    /// ViewModel class used for visualizing the information associated with the mini basket after refresh.
    /// </summary>
    public class MiniBasketRefreshViewModel
    {
        public string NumberOfItems { get; set; }
        public string Total { get; set; }
        public bool IsEmpty { get; set; }
        public string CartPageUrl { get; set; }

    }
}
