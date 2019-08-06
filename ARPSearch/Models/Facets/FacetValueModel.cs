using System;
using System.Runtime.Serialization;

namespace ARPSearch.Models.Facets
{
    [DataContract]
    public class FacetValueModel
    {
        [DataMember]
        public int AgregateCount { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return String.Format("Name: {0}; Value: {1}", Name, Value);
        }
    }
}