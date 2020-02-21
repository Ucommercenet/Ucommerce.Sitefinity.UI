using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Mvc.Model
{
    /// <summary>
    /// The contract to resolve the Model of the Confirmation Email MVC widget.
    /// </summary>
    public interface ICommentsModel
    {
        bool CanProcessRequest(Dictionary<string, object> parameters, out string message);
    }
}
