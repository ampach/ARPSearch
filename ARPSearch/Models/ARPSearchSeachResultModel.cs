using System.Collections.Generic;
using System.Runtime.Serialization;
using ARPSearch.Models.Base;

namespace ARPSearch.Models
{
    [DataContract]
    public class ARPSearchSeachResultModel : BaseSearchResultModel
    {
        [DataMember]
        public List<BaseIndexModel> Results { get; set; }
    }
}