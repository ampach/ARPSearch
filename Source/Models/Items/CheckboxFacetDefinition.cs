using ARPSearch.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class CheckboxFacetDefinition : FacetDefinition
    {
        public const string TemplateName = "Bool Facet Definition";
        public const string CheckboxLabelFieldName = "Checkbox Label";

        public CheckboxFacetDefinition(Item sourceItem) : base(sourceItem)
        {
            Init(sourceItem);
        }

        public string CheckboxLabel { get; set; }

        private void Init(Item sourceItem)
        {
            CheckboxLabel = sourceItem.GetStringValue(CheckboxLabelFieldName);
        }
    }
}