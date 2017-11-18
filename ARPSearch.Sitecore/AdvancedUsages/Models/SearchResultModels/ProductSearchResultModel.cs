using System.Collections.Generic;
using System.Runtime.Serialization;
using ARPSearch.Usages.AdvancedUsages.Models.ViewModels;

namespace ARPSearch.Usages.AdvancedUsages.Models.SearchResultModels
{
    public class ProductSearchResultModel : ARPSearch.Models.Base.BaseSearchResultModel
    {
        [DataMember]
        public List<ProductViewModel> Results { get; set; }
    }
}