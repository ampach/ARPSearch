using Sitecore.Diagnostics;

namespace ARPSearch.Models
{
    public class KeyValue
    {
        public string Key { get; protected set; }

        public string Value { get; protected set; }

        public KeyValue(string key, string value)
        {
            Assert.ArgumentNotNull((object)key, "key");
            Assert.ArgumentNotNull((object)value, "value");
            this.Key = key;
            this.Value = value;
        }
    }
}