using System;
using System.Collections.Generic;
using System.Web;
using UCommerce.Sitefinity.UI.Pages;
using Ucommerce.Search.Facets;
using Ucommerce.Infrastructure;
using Ucommerce.Api;
using System.Linq;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace UCommerce.Sitefinity.UI.Search
{
    public static class FacetedQueryStringExtensions
    {
        public static IList<Facet> ToFacets(this NameValueCollection target)
        {
            // TODO: revist this to identify if the indexdefinition is correct
            var productDefinition = ObjectFactory.Instance.Resolve<Ucommerce.Search.IIndexDefinition<Ucommerce.Search.Models.Product>>();

            var facets = productDefinition.Facets.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())).ToDictionary(x => x.Key, x => x.Value);
            string[] facetsKeys = new string[facets.Keys.Count];

            facets.Keys.CopyTo(facetsKeys, 0);

            var parameters = new Dictionary<string, string>();

            foreach (var queryString in HttpContext.Current.Request.QueryString.AllKeys)
            {
                parameters[queryString] = HttpContext.Current.Request.QueryString[queryString];
            }

            parameters.RemoveAll(p => !facetsKeys.Contains(p.Key));

            var facetsForQuerying = new List<Facet>();

            foreach (var parameter in parameters)
            {
                var facet = new Facet { FacetValues = new List<FacetValue>(), Name = parameter.Key };
                foreach (var value in parameter.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    facet.FacetValues.Add(new FacetValue() { Value = value });
                }


                facetsForQuerying.Add(facet);
            }

            return facetsForQuerying;
        }

        public static void RemoveAll<T>(this ICollection<T> list, Func<T, bool> predicate)
        {
            var matches = list.Where(predicate).ToArray();
            foreach (var match in matches)
            {
                list.Remove(match);
            }
        }
    }
}
