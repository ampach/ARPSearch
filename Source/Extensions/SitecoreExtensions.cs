using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ARPSearch.Extensions
{
    public static class SitecoreExtensions
    {
        
        public static string GetStringValue(this Item item, string fieldName)
        {
            var field = item.GetField(fieldName);

            return field == null || !field.HasValue ? null : field.Value;
        }
        public static int GetIntegerValue(this Item item, string fieldName)
        {
            var field = item.GetField(fieldName);

            if (field == null) return 0;

            var result = 0;
            int.TryParse(field.Value, out result);

            return result;
        }
        public static ID GetIDValue(this Item item, string fieldName)
        {
            var field = item.GetField(fieldName);

            if (field == null) return ID.Null;

            var referenceField = (Sitecore.Data.Fields.ReferenceField)field;

            if (referenceField != null)
            {
                return referenceField.TargetID;
            }

            return ID.Null;
        }
        public static bool GetBoolValue(this Item item, string fieldName, bool defaultValue = false)
        {
            var field = item.GetField(fieldName);

            if (field == null) return defaultValue;

            var checkboxField = (Sitecore.Data.Fields.CheckboxField)field;

            if (checkboxField != null)
            {
                return checkboxField.Checked;
            }

            return defaultValue;
        }
        public static Item GetItemValue(this Item item, string fieldName)
        {
            var field = item.GetField(fieldName);

            if (field == null) return null;

            var referenceField = (Sitecore.Data.Fields.ReferenceField)field;

            if (referenceField != null)
            {
                return referenceField.TargetItem;
            }

            return null;
        }
        public static List<Item> GetItemsListValue(this Item item, string fieldName)
        {
            var result = new List<Item>();

            var field = item.GetField(fieldName);

            if (field == null) return result;

            var multilistField = (Sitecore.Data.Fields.MultilistField)field;

            if (multilistField != null)
            {
                return multilistField.GetItems().ToList();
            }

            return result;
        }
        public static List<ID> GetIdsListValue(this Item item, string fieldName)
        {
            var result = new List<ID>();

            var field = item.GetField(fieldName);

            if (field == null) return result;

            var multilistField = (Sitecore.Data.Fields.MultilistField)field;

            if (multilistField != null)
            {
                return multilistField.TargetIDs.ToList();
            }

            return result;
        }

        public static Field GetField(this Item item, string fieldName)
        {
            if (item == null) return null;

            return item.Fields[fieldName];
        }
    }
}