using System.Collections.Generic;
using Sitecore.Data;

namespace ARPSearch.Models.Items.Interfaces
{
    public interface ISearchConfiguration
    {
        string IndexName { get; set; }
        List<ID> SearchRoots { get; set; }
        List<ID> IncludedTemplates { get; set; }
        List<FacetDefinition> Facets { get; set; }
        bool IsPaginated { get; set; }
        int ResultsPerPage { get; set; }
        bool LoadQueryString { get; set; }
        SearchServiceDefinition SearchServiceDefinition { get; set; }
    }
}