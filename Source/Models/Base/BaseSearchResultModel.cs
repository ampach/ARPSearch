using System.Collections.Generic;
using System.Runtime.Serialization;
using ARPSearch.Enums;
using ARPSearch.Models.Facets;

namespace ARPSearch.Models.Base
{
    [DataContract]
    public abstract class BaseSearchResultModel
    {
        public BaseSearchResultModel()
        {
            SearchResultType = ResultTypes.Undefined;
        }
        
        [DataMember]
        public IEnumerable<FacetModel> Facets { get; set; }
        
        [DataMember]
        public int TotalResult { get; set; }

        [DataMember]
        public ResultTypes SearchResultType { get; set; }

        
    }
}