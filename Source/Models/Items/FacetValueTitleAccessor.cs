using ARPSearch.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class FacetValueTitleAccessor
    {
        public const string FacetTitleFieldFieldName = "Facet Title Field";

        public FacetValueTitleAccessor(Item sourceItem)
        {
            Init(sourceItem);
        }

        public Item FacetTitleField { get; set; }

        private void Init(Item sourceItem)
        {
            FacetTitleField = sourceItem.GetItemValue(FacetTitleFieldFieldName);
        }
    }
}