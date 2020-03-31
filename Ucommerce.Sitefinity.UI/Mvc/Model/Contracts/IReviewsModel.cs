using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Contracts
{
    /// <summary>
    /// The contract to resolve the Model of the Confirmation Email MVC widget.
    /// </summary>
    public interface IReviewsModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ProductReviewsRenderingViewModel GetReviews(int? productId);
    }
}
