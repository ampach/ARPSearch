using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ARPSearch.Models.Facets
{
    [DataContract]
    public class FacetModel
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string FieldName { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public bool IsHasSelected { get; set; }
        [DataMember]
        public int SortOrder { get; set; }
        [DataMember]
        public string ViewType { get; set; }
        [DataMember]
        public List<FacetValueModel> Values { get; set; }

        public override string ToString()
        {
            return String.Format("FieldName: '{0}'; ValuesCount: {1}; Values: {2}", FieldName, Values != null ? Values.Count : 0, Values != null ? string.Join(" | ", Values) : "");
        }
    }
}