using System;

namespace ARPSearch.Models
{
    public class SearchAjaxRequest : ARPSearch.Models.SearchRequestModel
    {
        public Guid confID { get; set; }
        public string searchService { get; set; }
        public string searchResult { get; set; }
    }
}