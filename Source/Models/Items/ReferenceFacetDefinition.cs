using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class ReferenceFacetDefinition : FacetDefinition
    {
        public const string TemplateName = "Reference Facet Definition";

        public ReferenceFacetDefinition(Item sourceItem) : base(sourceItem)
        {
            Init(sourceItem);
        }

        public FacetValueTitleAccessor ValueTitleAccessor { get; set; }

        private void Init(Item sourceItem)
        {
            ValueTitleAccessor = new FacetValueTitleAccessor(sourceItem);
        }
    }
}