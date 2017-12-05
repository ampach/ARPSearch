using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ARPSearch.Models;
using ARPSearch.Models.Base;
using ARPSearch.Models.Items.Interfaces;
using ARPSearch.Service.Base;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace ARPSearch.Service
{
    public sealed class Search : SearchAbstractService<SearchRequestModel, BaseIndexModel, ARPSearchSeachResultModel>
    {
        public Search()
        {
        }
        public Search(ISearchConfiguration searchConfiguration) : base(searchConfiguration)
        {
        }

        protected override void MapSearchResults(ARPSearchSeachResultModel resultModel, SearchResults<BaseIndexModel> searchResultModel)
        {
            var urlOptions = LinkManager.GetDefaultUrlOptions();
            urlOptions.LanguageEmbedding = LanguageEmbedding.AsNeeded;

            var result = new List<BaseIndexModel>();
            foreach (var model in searchResultModel.Hits.Select(q => q.Document))
            {
                var item = model.GetItem();
                if (item == null)
                {
                    model.ItemUrl = model.Url;
                }
                else
                {
                    model.ItemUrl = item.Paths.IsMediaItem
                        ? MediaManager.GetMediaUrl((MediaItem)item)
                        : Sitecore.Links.LinkManager.GetItemUrl(item, urlOptions);
                }
                

                result.Add(model);
            }
            resultModel.Results = result;
        }
    }
}