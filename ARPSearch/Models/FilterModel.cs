using System.Runtime.Serialization;

namespace ARPSearch.Models
{
    [DataContract]
    public class FilterModel
    {
        [DataMember]
        public string FieldName { get; set; }
        [DataMember]
        public string FieldValue { get; set; }
    }
}