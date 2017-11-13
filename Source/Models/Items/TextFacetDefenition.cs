using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class TextFacetDefenition : FacetDefinition
    {
        public const string TemplateName = "Text Facet Defenition";

        public TextFacetDefenition(Item sourceItem) : base(sourceItem)
        {

        }
    }
}