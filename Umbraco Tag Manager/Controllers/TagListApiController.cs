using Microsoft.AspNetCore.Mvc;
using Our.Umbraco.Community.TagManager.Repositories;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace Our.Umbraco.Community.TagManager.Controllers
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