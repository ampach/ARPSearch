using System;
using System.Reflection;
using System.Web.Mvc;
using ARPSearch.Models;
using ARPSearch.Service;
using ARPSearch.Service.Base;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Mvc.Controllers;

namespace ARPSearch.Usages.Controllers
{
    public class ARPSearchController : SitecoreController
    {
        private readonly Search _searchService = new Search();

        public ActionResult SearchResults()
        {
            var configuration =
                ARPSearch.Models.Items.SearchConfiguration.Create(
                    Sitecore.Context.Database.GetItem(new Sitecore.Data.ID("{614BF2AA-3632-4CB1-B0A1-7364FA537490}")));

            return base.View();
        }

        [HttpPost]
        public string GetSearchResults(SearchAjaxRequest request)
        {
            try
            {
                if (request.confID == Guid.Empty || String.IsNullOrWhiteSpace(request.searchService))
                {
                    Logging.Log.Error("AJAX Request parameters are not populated.");
                    return "error";
                }

                Type serviceType = Type.GetType(request.searchService);

                if (serviceType == null)
                {
                    Logging.Log.Error("Service type cannot be resolved or is not inherit the ARPSearch.Servicebase.Search AbstractService<,,> class");
                    return "error";
                }
                var serviceInstance = Activator.CreateInstance(serviceType);

                MethodInfo methodInfo = serviceType.GetMethod("Search", new[] { typeof(ARPSearch.Models.SearchRequestModel), typeof(ARPSearch.Models.Items.Interfaces.ISearchConfiguration) });

                if (methodInfo != null)
                {
                    var parametersArray = new object[] { request, ARPSearch.Models.Items.SearchConfiguration.Create(Sitecore.Context.Database.GetItem(new ID(request.confID))) };
                    
                     var result = methodInfo.Invoke(serviceInstance, parametersArray);

                    var output = JsonConvert.SerializeObject(result);

                    return JsonConvert.SerializeObject(result); 
                }

                return "error";
            }
            catch (Exception e)
            {
                Logging.Log.Error("Getting Search Result: something was wrong.", e);
                return "error";
            }
            
        }
    }

}