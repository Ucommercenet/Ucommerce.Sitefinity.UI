using System;

namespace Ucommerce.Sitefinity.UI.Api.Model
{
    public class FullTextModel
    {
        public string SearchQuery { get; set; }
        public Guid PageId { get; set; }

        public int CategoryId { get; set; }
    }
}