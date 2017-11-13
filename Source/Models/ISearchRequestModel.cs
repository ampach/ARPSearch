using System.Collections.Generic;
using ARPSearch.Enums;
using ARPSearch.Models.Facets;

namespace ARPSearch.Models
{
    public interface ISearchRequestModel
    {
        string LastChangedFilterName { get; set; }

        List<FilterModel> Filters { get; set; }
        string SearchBoxQuery { get; set; }
        int Page { get; set; }
    }
}