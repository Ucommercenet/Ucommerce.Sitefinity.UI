using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    class CommentsModel : ICommentsModel
    {
        public bool CanProcessRequest(Dictionary<string, object> parameters, out string message)
        {
            if (Telerik.Sitefinity.Services.SystemManager.IsDesignMode)
            {
                message = "The widget is in Page Edit mode.";
                return false;
            }

            message = null;
            return true;
        }
    }
}
