using ARPSearch.Usages.AdvancedUsages.Services;

namespace ARPSearch.Usages.BasicUsages
{
    public class Examples
    {
        public void SetSearchConfigurationByConstructor()
        {
            var configurationItem = Sitecore.Context.Database.GetItem("{Search Configuration Item ID}");
            var searchConfiguration = ARPSearch.Models.Items.SearchConfiguration.Create(configurationItem);

            var searchService = new ARPSearch.Service.Search(searchConfiguration);

            var searchResults1 = searchService.Search();

            var requestModel = new ARPSearch.Models.SearchRequestModel();
            var searchResults2 = searchService.Search(requestModel);


        }

        public void SetSearchConfigurationBySearchMethod()
        {
            var searchService = new ARPSearch.Service.Search();

            var configurationItem = Sitecore.Context.Database.GetItem("{Search Configuration Item ID}");
            var searchConfiguration = ARPSearch.Models.Items.SearchConfiguration.Create(configurationItem);

            var searchResults1 = searchService.Search(searchConfiguration);

            var requestModel = new ARPSearch.Models.SearchRequestModel();
            var searchResults2 = searchService.Search(requestModel, searchConfiguration);

        }
    }
}