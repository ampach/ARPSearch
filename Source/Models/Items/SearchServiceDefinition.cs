using ARPSearch.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class SearchServiceDefinition
    {
        public const string SearchServiceReferenceFieldName = "Search Service Reference";

        public SearchServiceDefinition(Item sourceItem)
        {
            Init(sourceItem);
        }

        public string SearchServiceReference { get; set; }

        private void Init(Item sourceItem)
        {
            SearchServiceReference = sourceItem.GetStringValue(SearchServiceReferenceFieldName);
        }
    }
}