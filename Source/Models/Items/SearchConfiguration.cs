using System.Collections.Generic;
using System.Linq;
using ARPSearch.Extensions;
using ARPSearch.Factories;
using ARPSearch.Models.Items.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class SearchConfiguration : BaseItem, ISearchConfiguration
    {
        public const string SearchConfigurationTempalteID = "{4EAC136A-7E9A-4358-9F81-5DA5F6A67449}";

        public const string IndexNameFieldName = "Index Name";
        public const string SearchRootsFieldName = "Search Roots";
        public const string IncludedTemplatesFieldName = "Included Templates";
        public const string FacetsFieldName = "Facets";
        public const string IsPaginatedFieldName = "Is Paginated";
        public const string ResultsPerPageFieldName = "Results Per Page";
        public const string LoadQueryStringFieldName = "Load QueryString";
        public const string SearchServiceDefenitionFieldName = "Search Service Defenition";

        private SearchConfiguration(Item sourceItem) : base(sourceItem)
        {

            IndexName = sourceItem.Parent[IndexNameFieldName];
            SearchRoots = sourceItem.GetIdsListValue(SearchRootsFieldName);
            IncludedTemplates = sourceItem.GetIdsListValue(IncludedTemplatesFieldName);
            Facets = sourceItem.GetItemsListValue(FacetsFieldName).Select(FacetDefinitionFactory.Create)
                .Where(q => q != null).ToList();
            IsPaginated = sourceItem.GetBoolValue(IsPaginatedFieldName);
            ResultsPerPage = sourceItem.GetIntegerValue(ResultsPerPageFieldName);
            LoadQueryString = sourceItem.GetBoolValue(LoadQueryStringFieldName);

            var searchServiceDefenition = sourceItem.GetItemValue(SearchServiceDefenitionFieldName);
            SearchServiceDefinition = searchServiceDefenition != null ? new SearchServiceDefinition(searchServiceDefenition) : null;
        }

        public static SearchConfiguration Create(Item sourceItem)
        {
            if(sourceItem.TemplateID == new ID(SearchConfigurationTempalteID))
                return new SearchConfiguration(sourceItem);
            else
                return null;
        }

        public string IndexName { get; set; }
        public List<ID> SearchRoots { get; set; }
        public List<ID> IncludedTemplates { get; set; }
        public List<FacetDefinition> Facets { get; set; }
        public bool IsPaginated { get; set; }
        public int ResultsPerPage { get; set; }
        public bool LoadQueryString { get; set; }
        public SearchServiceDefinition SearchServiceDefinition { get; set; }
}
}