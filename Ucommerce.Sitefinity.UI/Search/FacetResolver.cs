using System;
using System.Collections.Generic;
using System.Web;
using UCommerce.Sitefinity.UI.Pages;
using Ucommerce.Search.Facets;

namespace UCommerce.Sitefinity.UI.Search
{
    /// <summary>
    /// Class that contains functionality for <see cref="Facet"/> retrieval.
    /// </summary>
    internal class FacetResolver
    {
        public FacetResolver(IList<string> queryStringBlackList)
        {
            this.queryStringBlackList = queryStringBlackList ?? new List<string>();
        }

        public IList<Facet> GetFacetsFromQueryString()
        {
            var facetsForQuerying = new List<Facet>();
            var queryStringParameter = UrlResolver.GetQueryStringParameters(this.queryStringBlackList);

            foreach (var queryString in queryStringParameter)
            {
                var facet = new Facet
                {
                    Name = queryString.Key,
                    FacetValues = new List<FacetValue>(),
                };

                foreach (var value in queryString.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    facet.FacetValues.Add(new FacetValue() { Value = HttpUtility.UrlDecode(value) });
                }

                facetsForQuerying.Add(facet);
            }

            return facetsForQuerying;
        }

        private readonly IList<string> queryStringBlackList;
    }
}
