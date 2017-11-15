using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using ARPSearch.Models;

namespace ARPSearch.Helpers
{
    public static class SearchHelper
    {
        public static IEnumerable<KeyValue> ToKeyValues(this NameValueCollection collection)
        {
            if (collection != null)
            {
                for (int n = 0; n < collection.Count; ++n)
                    yield return new KeyValue(collection.GetKey(n), collection[n]);
            }
        }
    }
}