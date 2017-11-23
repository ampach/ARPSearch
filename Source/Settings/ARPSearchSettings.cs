using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace ARPSearch.Settings
{
    public class ARPSearchSettings : IARPSearchSettings
    {
        public ARPSearchSettings()
        {
            this.Facets = new Dictionary<Guid, string>();
        }

        //public Guid RootSitecoreItem { get; private set; }
        //public string LocationsLandingKey { get; private set; }

        public Dictionary<Guid, string> Facets { get; private set; }


        public void AddFacetDef(string key, System.Xml.XmlNode node)
        {
            AddFacetDef(node);
        }
        public void AddFacetDef(System.Xml.XmlNode node)
        {
            var guid = Sitecore.Xml.XmlUtil.GetAttribute("facetDefenitionID", node);
            var name = Sitecore.Xml.XmlUtil.GetChildValue("templateId", node);
            this.Facets.Add(new Guid(guid), name);
        }


        private static volatile IARPSearchSettings current;

        private static readonly object lockObject = new object();

        public static IARPSearchSettings Current
        {
            get
            {
                if (current == null)
                {
                    lock (lockObject)
                    {
                        if (current == null)
                        {
                            current = CreateNewSettings();
                        }
                    }
                }

                return current;
            }
        }

        private static IARPSearchSettings CreateNewSettings()
        {
            IARPSearchSettings settings = Factory.CreateObject("settings/arpSearchSettings", true) as IARPSearchSettings;
            Assert.IsNotNull(settings, "General Settings must be set in configuration!");
            return settings;
        }
    }
}