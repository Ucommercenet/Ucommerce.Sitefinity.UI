using System.Collections.Generic;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Model.Contracts
{
    public interface IAddReviewModel
    {
        AddReviewDTO Add(AddReviewSubmitModel viewModel);
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
    }
}
