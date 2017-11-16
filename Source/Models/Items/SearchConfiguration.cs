using System.Collections.Generic;
using System.Linq;
using ARPSearch.Extensions;
using ARPSearch.Factories;
using ARPSearch.Models.Items.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class SearchConfiguration : ISearchConfiguration
    {
        public const string IndexNameFieldName = "Index Name";
        public const string SearchRootsFieldName = "Search Roots";
        public const string IncludedTemplatesFieldName = "Included Templates";
        public const string FacetsFieldName = "Facets";
        public const string IsPaginatedFieldName = "Is Paginated";
        public const string ResultsPerPageFieldName = "Results Per Page";
        public const string LoadQueryStringFieldName = "Load QueryString";

        public SearchConfiguration(Item sourceItem)
        {
            IndexName = sourceItem.Parent[IndexNameFieldName];
            SearchRoots = sourceItem.GetIdsListValue(SearchRootsFieldName);
            IncludedTemplates = sourceItem.GetIdsListValue(IncludedTemplatesFieldName);
            Facets = sourceItem.GetItemsListValue(FacetsFieldName).Select(FacetDefinitionFactory.Create)
                .Where(q => q != null).ToList();
            IsPaginated = sourceItem.GetBoolValue(IsPaginatedFieldName);
            ResultsPerPage = sourceItem.GetIntegerValue(ResultsPerPageFieldName);
            LoadQueryString = sourceItem.GetBoolValue(LoadQueryStringFieldName);
        }


        public string IndexName { get; set; }
        public List<ID> SearchRoots { get; set; }
        public List<ID> IncludedTemplates { get; set; }
        public List<FacetDefinition> Facets { get; set; }
        public bool IsPaginated { get; set; }
        public int ResultsPerPage { get; set; }
        public bool LoadQueryString { get; set; }
    }
}