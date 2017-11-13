using System.Collections.Generic;
using ARPSearch.Models.Base;

namespace ARPSearch.Models
{
    public class ARPSearchSeachResultModel : BaseSearchResultModel
    {
        public List<BaseIndexModel> Results { get; set; }
    }
}