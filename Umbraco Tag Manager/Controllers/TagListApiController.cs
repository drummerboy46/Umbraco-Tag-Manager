using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Community.TagManager.Repositories;

namespace Umbraco.Community.TagManager.Controllers
{
    public class TagListController(ITagListRepository tagListRepository) : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public string[] GetTagsByGroup(string group)
        {
            return tagListRepository.GetTagsByGroup(group);
        }
    }
}