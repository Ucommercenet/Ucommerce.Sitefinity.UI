using System;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    public class FullTextModel
    {
        public string SearchQuery { get; set; }

        public Guid? ProductDetailsPageId { get; set; }
    }
}