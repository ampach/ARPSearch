using System.Runtime.Serialization;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace ARPSearch.Models.Base
{
    [DataContract]
    public class BaseIndexModel : SearchResultItem
    {
        [IgnoreIndexField]
        [DataMember]
        public string ItemUrl { get; set; }
    }
}