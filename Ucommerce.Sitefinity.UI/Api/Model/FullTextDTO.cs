using System;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for passing search arguments.
    /// </summary>
    public class FullTextDTO
    {
        public Guid? ProductDetailsPageId { get; set; }
        public string SearchQuery { get; set; }
    }
}
