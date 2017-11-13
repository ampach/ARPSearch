using System.Linq;
using ARPSearch.Models;
using ARPSearch.Models.Base;
using ARPSearch.Models.Items.Interfaces;
using ARPSearch.Service.Base;
using Sitecore.ContentSearch.Linq;

namespace ARPSearch.Service
{
    public class Search : SearchAbstractService<SearchRequestModel, BaseIndexModel, ARPSearchSeachResultModel>
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