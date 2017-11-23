﻿using System;
using System.Collections.Generic;
using ARPSearch.Models.Items.Interfaces;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;

namespace ARPSearch.Models.RenderingModels
{
    public class SearchResultRenderingModel : Sitecore.Mvc.Presentation.IRenderingModel
    {
        public bool IsInitialized { get; set; }
        public ID SearchConfigurationId { get; set; }
        public string SearchService { get; set; }
        public Dictionary<Guid, string> Facets { get; set; }

        

        public void Initialize(Rendering rendering)
        {
            var searchConfiguration = ARPSearch.Models.Items.SearchConfiguration.Create(rendering.Item);
            if (searchConfiguration == null || string.IsNullOrWhiteSpace(searchConfiguration.IndexName) ||
                searchConfiguration.SearchServiceDefinition == null || string.IsNullOrWhiteSpace(searchConfiguration.SearchServiceDefinition.SearchServiceReference))
            {
                Logging.Log.Error("The Search Model Can't be initialized");
                IsInitialized = false;
                return;
            }

            SearchConfigurationId = searchConfiguration.ItemId;
            SearchService = searchConfiguration.SearchServiceDefinition.SearchServiceReference;

            Facets = ARPSearch.Settings.ARPSearchSettings.Current.Facets;

            IsInitialized = true;
        }
    }
}