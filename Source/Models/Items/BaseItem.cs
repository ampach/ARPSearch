using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class BaseItem
    {
        public BaseItem(Item sourceItem)
        {
            ItemId = sourceItem.ID;
            TemplateId = sourceItem.TemplateID;
        }

        public ID ItemId { get; set; }
        public ID TemplateId { get; set; }
    }
}