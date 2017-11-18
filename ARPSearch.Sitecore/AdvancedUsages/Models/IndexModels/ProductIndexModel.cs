using System.Collections.Generic;
using System.ComponentModel;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.Data;

namespace ARPSearch.Usages.AdvancedUsages.Models.IndexModels
{
    public class ProductIndexModel : ARPSearch.Models.Base.BaseIndexModel
    {
        [IndexField("issold")]
        public virtual bool IsSold { get; set; }

        [IndexField("category")]
        [TypeConverter(typeof(IndexFieldEnumerableConverter))]
        public virtual IEnumerable<ID> ProductCategory { get; set; }

        [IndexField("manufacturer")]
        [TypeConverter(typeof(IndexFieldIDValueConverter))]
        public virtual ID ProductManufacturer { get; set; }
    }
}