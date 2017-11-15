using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ARPSearch.Enums;
using ARPSearch.Models;
using ARPSearch.Models.Base;
using ARPSearch.Models.Facets;
using ARPSearch.Models.Items;
using ARPSearch.Models.Items.Interfaces;
using Sitecore;
using Sitecore.Common;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Diagnostics;

namespace ARPSearch.Service.Base
{
    public abstract class SearchAbstractService<TRequest, TIndexModel, TResult>
        where TRequest : ISearchRequestModel, new()
        where TResult : BaseSearchResultModel, new()
        where TIndexModel : BaseIndexModel, new()
    {

        #region Properties

        protected string SearchIndexName
        {
            get
            {
                return SearchConfiguration.IndexName;
            }
        }

        private ISearchIndex _index;
        private ISearchIndex Index
        {
            get { return _index ?? (_index = ContentSearchManager.GetIndex(SearchIndexName)); }
        }

        protected ISearchConfiguration SearchConfiguration;

        #endregion

        #region Constructors

        protected SearchAbstractService() { }

        protected SearchAbstractService(ISearchConfiguration searchConfiguration)
        {
            SetIndexConfiguration(searchConfiguration);
        }

        #endregion

        public TResult Search(TRequest model, ISearchConfiguration searchConfiguration)
        {
            SetIndexConfiguration(searchConfiguration);
            return Search(model);
        }

        public TResult Search()
        {
            return Search(new TRequest());
        }

        public IEnumerable<FacetModel> GetFacets(TRequest model, ISearchConfiguration configuration)
        {
            try
            {
                Assert.IsNotNull(Index, "Index");
                Assert.IsNotNull(configuration, "configuration != null");
                SearchConfiguration = configuration;

                if (!EnsureIndexExistence(Index))
                {
                    return new List<FacetModel>();
                }

                using (IProviderSearchContext searchContext = Index.CreateSearchContext())
                {
                    var items = searchContext.GetQueryable<TIndexModel>(
                        new CultureExecutionContext(Context.Language.CultureInfo));

                    return GetFacets(items, model);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Error occurred during getting facets: Error message:" + ex.ToString(), ex);
            }
            return new List<FacetModel>();
        }


        #region Virual and abstract methods

        public virtual TResult Search(TRequest model)
        {
            var result = new TResult();

            try
            {
                Assert.IsNotNull(Index, "Index");
                Assert.IsNotNull(SearchConfiguration, "Search configuration is null");

                if (!EnsureIndexExistence(Index))
                {
                    return result;
                }

                using (IProviderSearchContext searchContext = Index.CreateSearchContext())
                {
                    try
                    {
                        var items = searchContext.GetQueryable<TIndexModel>(new CultureExecutionContext(Context.Language.CultureInfo));

                        result.Facets = GetFacets(items, model);

                        var globalPredicateBuilder = PredicateBuilder.True<TIndexModel>();

                        var predicate = globalPredicateBuilder.And(ApplyPredefinedFilters(model));

                        predicate = predicate.And(Filter(model));

                        items = items.Where(predicate);

                        items = ApplyOrdering(items);

                        if (SearchConfiguration.IsPaginated)
                        {
                            items = items.Page(model.Page < 1 ? 0 : model.Page - 1, SearchConfiguration.ResultsPerPage);
                        }

                        var searchResults = items.GetResults();

                        MapSearchResults(result, searchResults);

                        result.TotalResult = searchResults.TotalSearchResults;

                        result.SearchResultType = ResultTypes.Success;
                    }
                    catch (Exception ex)
                    {
                        result.SearchResultType = ResultTypes.Faild;
                        Logging.Log.Error(ex.ToString(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SearchResultType = ResultTypes.Faild;
                Logging.Log.Error(ex.ToString(), ex);
            }
            return result;
        }

        protected virtual Expression<Func<TIndexModel, bool>> ApplySpecificFilters(TRequest model)
        {
            return q => true;
        }
        protected abstract void MapSearchResults(TResult resultModel, SearchResults<TIndexModel> searchResultModel);

        protected virtual IQueryable<TIndexModel> ApplyOrdering(IQueryable<TIndexModel> query)
        {
            return query;
        }

        protected virtual IQueryable<TIndexModel> ApplyFacets(IQueryable<TIndexModel> query)
        {
            return query;
        }

        #endregion


        #region Private methods

        private IQueryable<TIndexModel> ApplyConfigurationFacets(IQueryable<TIndexModel> query)
        {
            if (SearchConfiguration.Facets != null && SearchConfiguration.Facets.Any())
            {
                query = SearchConfiguration.Facets.Aggregate(query, (current, facet) => current.FacetOn(w => w[facet.IndexFieldName]));
            }
            query = ApplyFacets(query);
            return query;
        }

        private IEnumerable<FacetModel> GetFacets(IQueryable<TIndexModel> query, TRequest model)
        {
            var result = new List<FacetModel>();

            var predicateFilterGlobal = ApplyPredefinedFilters(model);
            var allFacetBuilder = query.Where(predicateFilterGlobal);

            allFacetBuilder = ApplyConfigurationFacets(allFacetBuilder);

            var allFacets = allFacetBuilder.GetFacets().Categories.Select(ToCustomModel).ToList();

            if (model.Filters == null || !model.Filters.Any())
            {
                return allFacets.OrderBy(q => q.SortOrder).ToList();
            }

            var searchFilters = model.Filters.Distinct()
                .Where(f => !string.IsNullOrWhiteSpace(f.FieldName))
                .OrderBy(q => q.FieldName)
                .GroupBy(q => q.FieldName)
                .ToList();

            var lastChangedFilterName = model.LastChangedFilterName;
            if (string.IsNullOrWhiteSpace(lastChangedFilterName))
            {
                var lastChangedFilter = model.Filters.LastOrDefault();
                if (lastChangedFilter != null)
                {
                    lastChangedFilterName = model.LastChangedFilterName = lastChangedFilter.FieldName;
                }
            }

            if (searchFilters.Count > 0)
            {
                // Get Facets for all filters but the Latest Changed:

                var predicat = PredicateBuilder.True<TIndexModel>();
                foreach (var searchFilter in searchFilters)
                {
                    foreach (var filter in searchFilter)
                    {
                        predicat = predicat.And(q => q[filter.FieldName] == filter.FieldValue);
                    }
                }

                var expression = predicateFilterGlobal.And(predicat);
                var queryFacets = query.Where(expression);
                queryFacets = ApplyConfigurationFacets(queryFacets);
                var facets = queryFacets.GetFacets().Categories.Select(ToCustomModel).ToList();
                facets = facets.Where(f => f.FieldName != lastChangedFilterName).ToList();
                result.AddRange(facets);

                // Get Facets for the Latest Changed filter:

                predicat = PredicateBuilder.True<TIndexModel>();
                var i = 0;
                foreach (var groupFilter in searchFilters)
                {
                    if (groupFilter.Key != lastChangedFilterName)
                    {
                        foreach (var filter in groupFilter)
                        {
                            predicat = predicat.And(q => q[filter.FieldName] == filter.FieldValue);
                            i++;
                        }
                    }
                }

                if (i > 0)
                {
                    expression = predicateFilterGlobal.And(predicat);
                    queryFacets = query.Where(expression);
                    queryFacets = ApplyConfigurationFacets(queryFacets);
                    facets = queryFacets.GetFacets().Categories.Select(ToCustomModel).ToList();
                    facets = facets.Where(f => f.FieldName == lastChangedFilterName).ToList();
                    result.AddRange(facets);
                }
                else
                {
                    result.AddRange(allFacets.Where(f => f.FieldName == lastChangedFilterName).ToList());
                }
            }
            else
            {
                result.AddRange(allFacets);
            }

            var tempResults = new List<FacetModel>();
            var groupedResults = result.GroupBy(q => q.FieldName);
            foreach (var groupedResult in groupedResults)
            {
                var temp2 = new FacetModel();
                var temp2Values = new List<FacetValueModel>();
                foreach (var facetModel in groupedResult)
                {
                    temp2.FieldName = facetModel.FieldName;
                    temp2.Enabled = facetModel.Enabled;
                    temp2.Title = facetModel.Title;
                    temp2.SortOrder = facetModel.SortOrder;
                    temp2.ViewType = facetModel.ViewType;
                    foreach (var val in facetModel.Values)
                    {
                        var tempVal = new FacetValueModel
                        {
                            Name = val.Name,
                            Value = val.Value,
                            AgregateCount = val.AgregateCount,
                            IsSelected =
                                model.Filters.Any(
                                    f => f.FieldName == facetModel.FieldName && f.FieldValue == val.Value)
                        };
                        temp2Values.Add(tempVal);
                    }

                    temp2.IsHasSelected = temp2Values.Any(e => e.IsSelected);
                }

                temp2.Values = temp2Values.GroupBy(x => x.Value).Select(x => x.First()).ToList();
                tempResults.Add(temp2);
            }

            result = tempResults;

            return result.OrderBy(q => q.SortOrder);
        }

        private Expression<Func<TIndexModel, bool>> ApplyPredefinedFilters(TRequest model)
        {
            var predicateFilterMain = PredicateBuilder.True<TIndexModel>();

            //Filtering by the root items.
            if (SearchConfiguration.SearchRoots != null && SearchConfiguration.SearchRoots.Any())
            {
                var includedTemplatesPredicate = PredicateBuilder.False<TIndexModel>();
                foreach (var root in SearchConfiguration.SearchRoots)
                {

                    Expression<Func<TIndexModel, bool>> isUnderRoot = item => item.Paths.Contains(root);

                    includedTemplatesPredicate = includedTemplatesPredicate.Or(isUnderRoot);
                }

                predicateFilterMain = predicateFilterMain.And(includedTemplatesPredicate);
            }

            //Defining templates which would be included in the search result
            if (SearchConfiguration.IncludedTemplates != null && SearchConfiguration.IncludedTemplates.Any())
            {
                var includedTemplatesPredicate = PredicateBuilder.False<TIndexModel>();
                foreach (var includedTemplate in SearchConfiguration.IncludedTemplates)
                {
                    
                    Expression<Func<TIndexModel, bool>> isBasedOnTemplate = item => item.TemplateId == includedTemplate;

                    includedTemplatesPredicate = includedTemplatesPredicate.Or(isBasedOnTemplate);
                }

                predicateFilterMain = predicateFilterMain.And(includedTemplatesPredicate);
            }

            //Applying further specific filters that can be added in an inheriting class
            predicateFilterMain = predicateFilterMain.And(ApplySpecificFilters(model));

            return predicateFilterMain;
        }

        private Expression<Func<TIndexModel, bool>> Filter(TRequest model)
        {
            var predicateMain = PredicateBuilder.True<TIndexModel>();

            if (SearchConfiguration.Facets != null && SearchConfiguration.Facets.Any())
            {
                if (model.Filters != null && model.Filters.Any())
                {
                    var groupedFilters = model.Filters.GroupBy(q => q.FieldName);

                    foreach (var filterGroup in groupedFilters)
                    {
                        if (SearchConfiguration.Facets.Any(w => w.IndexFieldName == filterGroup.Key))
                        {
                            var filterPredicateBuilder = PredicateBuilder.False<TIndexModel>();
                            foreach (var filter in filterGroup)
                            {
                                filterPredicateBuilder = filterPredicateBuilder.Or(itm => itm[filterGroup.Key].Contains(filter.FieldValue) || itm[filterGroup.Key].Equals(filter.FieldValue));
                            }

                            predicateMain = predicateMain.And(filterPredicateBuilder);
                        }
                    }
                }
            }

            return predicateMain;
        }

        private bool EnsureIndexExistence(ISearchIndex index)
        {
            if(index == null)
            {
                Logging.Log.Error("Index is null. Check the name of index.");
                return false;
            }

            if (index.Summary.NumberOfDocuments == 0)
            {
                var message = string.Format("Index is empty: {0}. You should rebuild index.", index.Name);
                Logging.Log.Error(message);
                return true;
            }

            return true;
        }

        private FacetModel ToCustomModel(FacetCategory category)
        {
            var facetDefinition = GetFacetDefinition(category);

            if (facetDefinition == null)
            {
                return new FacetModel();
            }

            return new FacetModel
            {
                Enabled = true,
                FieldName = category.Name,
                SortOrder = facetDefinition.SortOrder,
                Title = !String.IsNullOrWhiteSpace(facetDefinition.Title) ? facetDefinition.Title : category.Name,
                ViewType = facetDefinition.FacetViewType,
                Values = category.Values.Where(q => q.AggregateCount > 0).Select(v => new FacetValueModel
                {
                    AgregateCount = v.AggregateCount,
                    Name = GetFacetName(facetDefinition, v.Name),
                    Value = v.Name
                }).Where(q => !String.IsNullOrWhiteSpace(q.Name)).OrderBy(v => v.Name).ToList()
            };
        }

        private string GetFacetName(FacetDefinition facetDefinition, string value)
        {
            if (facetDefinition is ReferenceFacetDefinition)
            {
                var referenceFacetDefinition = facetDefinition as ReferenceFacetDefinition;

                Guid id;
                if (Guid.TryParse(value, out id))
                {
                    if (id == Guid.Empty)
                    {
                        return String.Empty;
                    }

                    var item = Context.Database.GetItem(id.ToID());

                    if (item != null && referenceFacetDefinition.ValueTitleAccessor != null && referenceFacetDefinition.ValueTitleAccessor.FacetTitleField != null)
                    {
                        var field = item.Fields[referenceFacetDefinition.ValueTitleAccessor.FacetTitleField.Name];

                        if (field != null) return !String.IsNullOrWhiteSpace(field.Value) ? field.Value : item.Name; 
                    }
                }
            }

            if (facetDefinition is TextFacetDefenition)
            {
                return value;
            }

            return value;
        }

        private FacetDefinition GetFacetDefinition(FacetCategory category)
        {
            if (category == null || SearchConfiguration.Facets == null)
            {
                return null;
            }

            return SearchConfiguration.Facets.FirstOrDefault(q => q.IndexFieldName.Equals(category.Name));
        }

        private void SetIndexConfiguration(ISearchConfiguration searchConfiguration)
        {
            Assert.IsNotNull(searchConfiguration, "Search configuration is null");

            SearchConfiguration = searchConfiguration;
        }

        #endregion
    }
}