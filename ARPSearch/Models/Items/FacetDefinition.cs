using System;
using ARPSearch.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ARPSearch.Models.Items
{
    public class FacetDefinition : BaseItem
    {
        public FacetDefinition(Item sourceItem) : base(sourceItem)
        {
            Init(sourceItem);
        }

        public const string TitleFieldName = "Title"; 
        public const string IndexFieldNameFieldName = "Index Field Name"; 
        public const string SortOrderFieldName = "Sort Order"; 
        public const string FacetViewTypeFieldName = "Facet View Type"; 

        public string Title { get; set; }
        public string IndexFieldName { get; set; }
        public int SortOrder { get; set; }
        public string FacetViewType { get; set; }

        private void Init(Item sourceItem)
        {
            Title = sourceItem.GetStringValue(TitleFieldName);
            IndexFieldName = sourceItem.GetStringValue(IndexFieldNameFieldName);
            SortOrder = sourceItem.GetIntegerValue(SortOrderFieldName);

            FacetViewType = sourceItem.GetIDValue(FacetViewTypeFieldName).ToShortID().ToString();
        }
    }
}