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
            InnerItem = sourceItem;
        }

        public ID ItemId { get; set; }
        public ID TemplateId { get; set; }
        public Item InnerItem { get; set; }
    }
}