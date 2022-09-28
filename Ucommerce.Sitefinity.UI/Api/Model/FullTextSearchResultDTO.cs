﻿namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for storing search result.
    /// </summary>
    public class FullTextSearchResultDTO
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string Url { get; set; }
    }
}
