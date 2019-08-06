using ARPSearch.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class ReferenceFacetDefinition : FacetDefinition
    {
        public const string TemplateName = "Reference Facet Definition";
        public const string FacetTitleFieldFieldName = "Value Title Accessor";

        public ReferenceFacetDefinition(Item sourceItem) : base(sourceItem)
        {
            Init(sourceItem);
        }

        public FacetValueTitleAccessor ValueTitleAccessor { get; set; }

        private void Init(Item sourceItem)
        {
            var valueTitleAccessorItem = sourceItem.GetItemValue(FacetTitleFieldFieldName);

            ValueTitleAccessor = valueTitleAccessorItem != null ? new FacetValueTitleAccessor(valueTitleAccessorItem) : null;
        }
    }
}