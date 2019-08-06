using ARPSearch.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class SearchServiceDefinition
    {
        public const string SearchServiceReferenceFieldName = "Search Service Reference";
        public const string SearchResultModelFieldName = "Search Result Model";

        public SearchServiceDefinition(Item sourceItem)
        {
            Init(sourceItem);
        }

        public string SearchServiceReference { get; set; }
        public string SearchResultModel { get; set; }

        private void Init(Item sourceItem)
        {
            SearchServiceReference = sourceItem.GetStringValue(SearchServiceReferenceFieldName);
            SearchResultModel = sourceItem.GetStringValue(SearchResultModelFieldName);
        }
    }
}