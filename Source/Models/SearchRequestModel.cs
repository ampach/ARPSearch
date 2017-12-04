using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ARPSearch.Models
{
    public class SearchRequestModel : ISearchRequestModel
    {
        [DataMember]
        public string LastChangedFilterName { get; set; }

        [DataMember]
        public List<FilterModel> Filters { get; set; }

        [DataMember]
        public string SearchBoxQuery { get; set; }
        [DataMember]
        public string CurrentUrl { get; set; }

        [DataMember]
        public int Page { get; set; }
    }
}