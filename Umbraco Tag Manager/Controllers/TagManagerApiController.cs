using System.Collections.Generic;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Community.TagManager.Models;
using Umbraco.Community.TagManager.Repositories;

namespace Umbraco.Community.TagManager.Controllers
{
    [PluginController(StringConstants.PluginAlias)]
    public class TagManagerApiController(ITagManagerRepository tagManagerRepository) : UmbracoAuthorizedJsonController
    {
        public TagList GetTagsById(int id)
        {
            return tagManagerRepository.GetTagsById(id);
        }

        public TagList GetTagsByGroup(string group)
        {
            return tagManagerRepository.GetTagsByGroup(group);
        }

        public List<TagGroup> GetTagGroups()
        {
            return tagManagerRepository.GetTagGroups();
        }

        public PagedContent GetPagedContent(int id, int offset, int limit)
        {
            return tagManagerRepository.GetPagedContent(id, offset, limit);
        }

        public PagedMedia GetPagedMedia(int id, int offset, int limit)
        {
            return tagManagerRepository.GetPagedMedia(id, offset, limit);
        }

        public int CreateGroup(string group)
        {
            return tagManagerRepository.CreateGroup(group);
        }

        public int CreateTag(TagItem tags)
        {
            return tagManagerRepository.CreateTag(tags);
        }

        public int SaveTag(TagList tags)
        {
            return tagManagerRepository.SaveTag(tags);
        }

        public int DeleteTag(TagList tags)
        {
            return tagManagerRepository.DeleteTag(tags);
        }

        public int DeleteTags(TagList tags)
        {
            return tagManagerRepository.DeleteTags(tags);
        }
    }
}