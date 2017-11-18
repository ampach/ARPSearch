using System;
using System.Linq;
using System.Linq.Expressions;
using ARPSearch.Models;
using ARPSearch.Models.Base;
using ARPSearch.Models.Items.Interfaces;
using ARPSearch.Service.Base;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;

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
            resultModel.Results = searchResultModel.Hits.Select(q => q.Document).ToList();
        }
    }
}