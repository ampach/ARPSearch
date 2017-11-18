using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ARPSearch.Enums;
using ARPSearch.Helpers;
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
    /// <summary>
    /// Abstract class with base implementation of search.
    /// </summary>
    /// <typeparam name="TRequest">Search Request Model. It has to implement ISearchRequestModel interface.</typeparam>
    /// <typeparam name="TIndexModel">Model which will be returned from index.</typeparam>
    /// <typeparam name="TResult">Model that includes the search result with additional data like facets, count of results.</typeparam>
    public abstract class SearchAbstractService<TRequest, TIndexModel, TResult>
        where TRequest : ISearchRequestModel, new()
        where TResult : BaseSearchResultModel, new()
        where TIndexModel : BaseIndexModel, new()
    {

        #region Properties

        /// <summary>
        /// Index Name. Value is coming from Sitecore. 
        /// </summary>
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
            get
            {
                if (_index == null || _index.Name != SearchIndexName)
                {
                    _index = ContentSearchManager.GetIndex(SearchIndexName);
                }

                return _index;
            }
        }


        protected ISearchConfiguration SearchConfiguration;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        protected SearchAbstractService() { }

        /// <summary>
        /// The constructor can be used if we want to set a Search Configuration during creating instance of  search service
        /// </summary>
        /// <param name="searchConfiguration">Instance of Search Configuration class</param>
        protected SearchAbstractService(ISearchConfiguration searchConfiguration)
        {
            SetIndexConfiguration(searchConfiguration);
        }

        #endregion

        /// <summary>
        /// Search without a request model. May be used when you don't have any filtering parameters for search.
        /// </summary>
        /// <returns>Result of search</returns>
        public TResult Search()
        {
            return Search(new TRequest());
        }

        /// <summary>
        /// Search with passing a Search configuration but without a request model. May be used when you want to use different Search Configuration with 
        /// the same instance of search service and you don't have any filtering parameters for search. Search Configuration is required.
        /// </summary>
        /// <param name="searchConfiguration">Instance of Search Configuration class</param>
        /// <returns>Result of search</returns>
        public TResult Search(ISearchConfiguration searchConfiguration)
        {
            return Search(new TRequest(), searchConfiguration);
        }

        /// <summary>
        /// Search with passing a Request Model. A search configuration which has defined by constructor or during previous search request will be used. 
        /// </summary>
        /// <param name="requestModel">Request Model</param>
        /// <returns>Result of search</returns>
        public TResult Search(TRequest requestModel)
        {
            return Search(requestModel, null);
        }
        
        /// <summary>
        /// Search with passing a Search configuration and a request model. May be used when you want to use different Search Configuration with 
        /// the same instance of search service and you have filtering parameters for search. Search Configuration is required.
        /// </summary>
        /// <param name="requestModel">Request Model</param>
        /// <param name="searchConfiguration">Instance of Search Configuration class</param>
        /// <returns>Result of search</returns>
        public TResult Search(TRequest requestModel, ISearchConfiguration searchConfiguration)
        {
            var result = new TResult();

            try
            {
                if (searchConfiguration != null)
                {
                    SetIndexConfiguration(searchConfiguration);
                }

                Assert.IsNotNull(Index, "Search Index is null");
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

                        PopulateFromQueryString(requestModel);

                        result.Facets = GetFacets(items, requestModel);

                        var globalPredicateBuilder = PredicateBuilder.True<TIndexModel>();

                        var predicate = globalPredicateBuilder.And(ApplyPredefinedFilters(requestModel));

                        predicate = predicate.And(Filter(requestModel));

                        items = items.Where(predicate);

                        items = ApplyOrdering(items);

                        if (SearchConfiguration.IsPaginated)
                        {
                            items = items.Page(requestModel.Page < 1 ? 0 : requestModel.Page - 1, SearchConfiguration.ResultsPerPage);
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

        
        /// <summary>
        /// Method is used for getting direct facets without getting search results
        /// A search configuration which has defined by constructor or during previous search request will be used.
        /// </summary>
        /// <param name="requestModel">Request Models</param>
        /// <returns>List of facets</returns>
        public IEnumerable<FacetModel> GetFacets(TRequest requestModel)
        {
            return GetFacets(requestModel, null);
        }

        /// <summary>
        /// Method is used for getting direct facets without getting search results
        /// </summary>
        /// <param name="requestModel">Request Models</param>
        /// <param name="searchConfiguration">Search Configuration (Required)</param>
        /// <returns>List of facets</returns>
        public IEnumerable<FacetModel> GetFacets(TRequest requestModel, ISearchConfiguration searchConfiguration)
        {
            try
            {
                if (searchConfiguration != null)
                {
                    SetIndexConfiguration(searchConfiguration);
                }

                Assert.IsNotNull(Index, "Index");
                Assert.IsNotNull(SearchConfiguration, "SearchConfiguration is null");

                if (!EnsureIndexExistence(Index))
                {
                    return new List<FacetModel>();
                }

                using (IProviderSearchContext searchContext = Index.CreateSearchContext())
                {
                    var items = searchContext.GetQueryable<TIndexModel>(
                        new CultureExecutionContext(Context.Language.CultureInfo));

                    return GetFacets(items, requestModel);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Error occurred during getting facets: Error message:" + ex.ToString(), ex);
            }
            return new List<FacetModel>();
        }


        #region Virual and abstract methods

        /// <summary>
        /// Used for applying additional conditions to query. IMPORTANT! This method is executed during building a query for search and shouldn't be called in another places.
        /// Currently there is defined a filtering by a search query. It can be overridden in a class which will inherit current class.
        /// </summary>
        protected virtual Expression<Func<TIndexModel, bool>> ApplySpecificFilters(TRequest requestModel)
        {
            if (!String.IsNullOrWhiteSpace(requestModel.SearchBoxQuery))
            {
                var query = requestModel.SearchBoxQuery;

                var predicate = PredicateBuilder.False<TIndexModel>();

                predicate = predicate.Or(item => item.Name.Contains(query).Boost(1.5f));
                predicate = predicate.Or(item => item.Content.Contains(query));

                return predicate;
            }

            return q => true;
        }

        /// <summary>
        /// Used for mapping an Index model to a Search Result Model. IMPORTANT! This method is executed during building a query for search and shouldn't be called in another places.
        /// It have to be implemented in a class which will inherit current class.
        /// </summary>
        protected abstract void MapSearchResults(TResult resultModel, SearchResults<TIndexModel> searchResultModel);

        /// <summary>
        /// Used for applying extra ordering to a search results. IMPORTANT! This method is executed during building a query for search and shouldn't be called in another places.
        /// By default, a search results are ordrered by rank but It can be overridden in a class which will inherit current class.
        /// </summary>
        protected virtual IQueryable<TIndexModel> ApplyOrdering(IQueryable<TIndexModel> query)
        {
            return query;
        }

        /// <summary>
        /// Currently, the facets are defined in the Sitecore. If you need to add any extra facets from code - just override this method.
        /// IMPORTANT! This method is executed during building a query for search and shouldn't be called in another places.
        /// </summary>
        protected virtual IQueryable<TIndexModel> ApplyFacets(IQueryable<TIndexModel> query)
        {
            return query;
        }


        /// <summary>
        /// Method is used for populating a request model from Query String parameters. If you need to add populating any extra parameters - just override this method.
        /// IMPORTANT! This method is executed during building a query for search and shouldn't be called in another places.
        /// </summary>
        protected virtual void PopulateFromQueryString(ISearchRequestModel model)
        {
            if (!SearchConfiguration.LoadQueryString)
            {
                Logging.Log.Debug("Loading search parameters from query string is disabled");
                return;
            }

            if (HttpContext.Current == null)
            {
                Logging.Log.Debug("Loading search parameters from query string is impossible due the HttpContext is null.");
                return;
            }

            if (HttpContext.Current.Request.QueryString.Count < 1)
            {
                Logging.Log.Debug("There is no any query string parameters in the current request.");
                return;
            }

            var query = HttpContext.Current.Request.QueryString["q"];
            if (!string.IsNullOrWhiteSpace(query))
            {
                model.SearchBoxQuery = query;
            }

            var querystrings = HttpContext.Current.Request.QueryString.ToKeyValues().Where(q => q.Key != "q").ToList();
            if (!querystrings.Any())
                return;

            model.Filters = querystrings.Select(q => new FilterModel
            {
                FieldName = q.Key,
                FieldValue = q.Value
            }).ToList();

            var filter = model.Filters.LastOrDefault();
            if (filter != null)
            {
                model.LastChangedFilterName = filter.FieldName;
            }
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
            
            if (model.Filters != null && model.Filters.Any())
            {
                var groupedFilters = model.Filters.GroupBy(q => q.FieldName);

                foreach (var filterGroup in groupedFilters)
                {
                    var filterPredicateBuilder = PredicateBuilder.False<TIndexModel>();
                    foreach (var filter in filterGroup)
                    {
                        filterPredicateBuilder = filterPredicateBuilder.Or(itm => itm[filterGroup.Key].Contains(filter.FieldValue) || itm[filterGroup.Key].Equals(filter.FieldValue));
                    }

                    predicateMain = predicateMain.And(filterPredicateBuilder);
                    
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