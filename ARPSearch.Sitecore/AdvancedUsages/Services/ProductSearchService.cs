using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ARPSearch.Extensions;
using ARPSearch.Models;
using ARPSearch.Models.Items.Interfaces;
using ARPSearch.Usages.AdvancedUsages.Models.IndexModels;
using ARPSearch.Usages.AdvancedUsages.Models.SearchRequestModels;
using ARPSearch.Usages.AdvancedUsages.Models.SearchResultModels;
using ARPSearch.Usages.AdvancedUsages.Models.ViewModels;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;

namespace ARPSearch.Usages.AdvancedUsages.Services
{
    public class ProductSearchService : ARPSearch.Service.Base.SearchAbstractService<ProductSearchRequest, ProductIndexModel,ProductSearchResultModel>
    {
        public ProductSearchService()
        {
        }
        public ProductSearchService(ISearchConfiguration searchConfiguration) : base(searchConfiguration)
        {
        }

        protected override Expression<Func<ProductIndexModel, bool>> ApplySpecificFilters(ProductSearchRequest requestModel)
        {
            var predicate = PredicateBuilder.True<ProductIndexModel>();

            predicate = predicate.And(item => item.IsSold == requestModel.IsSold);

            predicate.And(base.ApplySpecificFilters(requestModel));

            return predicate;
        }

        protected override IQueryable<ProductIndexModel> ApplyOrdering(IQueryable<ProductIndexModel> query)
        {
            return query.OrderBy(q => q.Name);
        }

        protected override void MapSearchResults(ProductSearchResultModel resultModel, SearchResults<ProductIndexModel> searchResultModel)
        {
            var result = new List<ProductViewModel>();

            var searchResults = searchResultModel.Hits.Select(q => q.Document);
            foreach (var sr in searchResults)
            {
                var item = sr.GetItem();
                result.Add(new ProductViewModel
                {
                    Title = item.GetStringValue("Title"),
                    Category = item.GetItemsListValue("Category").Select(q => q.Name).Aggregate((first, second) => first + ", " + second),
                    Manufacturer = item.GetItemValue("Manufacturer").Name
                });
            }

            resultModel.Results = result;
        }
    }
}