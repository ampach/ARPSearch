using System.Collections.Generic;
using ARPSearch.Models;
using ARPSearch.Models.Base;
using ARPSearch.Models.Facets;
using ARPSearch.Models.Items.Interfaces;

namespace ARPSearch.Service.Base
{
    public interface ISearchService<in TRequest, out TResult>
        where TRequest : ISearchRequestModel, new()
        where TResult : BaseSearchResultModel, new()
    {
        TResult Search();
        TResult Search(ISearchConfiguration searchConfiguration);
        TResult Search(TRequest requestModel);
        TResult Search(TRequest requestModel, ISearchConfiguration searchConfiguration);
        IEnumerable<FacetModel> GetFacets(TRequest requestModel);
        IEnumerable<FacetModel> GetFacets(TRequest requestModel, ISearchConfiguration searchConfiguration);
    }
}