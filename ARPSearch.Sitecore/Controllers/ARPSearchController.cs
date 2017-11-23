using System.Web.Mvc;
using ARPSearch.Service;
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
        public JsonResult GetSearchResults(string itemId)
        {
            return Json("success");
        }
    }

}