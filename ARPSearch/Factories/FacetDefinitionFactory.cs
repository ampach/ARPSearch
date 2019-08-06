using System.Runtime.Remoting.Messaging;
using ARPSearch.Models.Items;
using Sitecore.Data.Items;

namespace ARPSearch.Factories
{
    public class FacetDefinitionFactory
    {
        public static FacetDefinition Create(Item item)
        {
            if (item == null) return null;

            switch (item.TemplateName)
            {
                case TextFacetDefenition.TemplateName: return new TextFacetDefenition(item);
                case ReferenceFacetDefinition.TemplateName: return new ReferenceFacetDefinition(item);
                case CheckboxFacetDefinition.TemplateName: return new CheckboxFacetDefinition(item);

                default: return null;
            }
        }
    }
}