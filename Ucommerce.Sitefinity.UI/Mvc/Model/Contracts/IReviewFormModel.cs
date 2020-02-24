using System.Collections.Generic;
using UCommerce.EntitiesV2;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Contracts
{
    public interface IReviewFormModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
        ProductReview Add(ReviewFormSaveViewModel viewModel);
    }
}