using System.Collections.Generic;
using Our.Umbraco.Community.TagManager.Models;
using Our.Umbraco.Community.TagManager.Repositories;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Our.Umbraco.Community.TagManager.Controllers
{
    [PluginController(StringConstants.PluginAlias)]
    public class TagManagerApiController : UmbracoAuthorizedJsonController
    {
        private readonly ITagManagerRepository _tagManagerRepository;

        public TagManagerApiController(ITagManagerRepository tagManagerRepository)
        {
            _tagManagerRepository = tagManagerRepository;
        }

        public TagList GetTagsById(int id)
        {
            return _tagManagerRepository.GetTagsById(id);
        }

        public TagList GetTagsByGroup(string group)
        {
            return _tagManagerRepository.GetTagsByGroup(group);
        }

        public List<TagGroup> GetTagGroups()
        {
            return _tagManagerRepository.GetTagGroups();
        }

        public PagedContent GetPagedContent(int id, int offset, int limit)
        {
            return _tagManagerRepository.GetPagedContent(id, offset, limit);
        }

        public PagedMedia GetPagedMedia(int id, int offset, int limit)
        {
            return _tagManagerRepository.GetPagedMedia(id, offset, limit);
        }

        public int CreateGroup(string group)
        {
            return _tagManagerRepository.CreateGroup(group);
        }

        public int CreateTag(TagItem tags)
        {
            return _tagManagerRepository.CreateTag(tags);
        }

        public int SaveTag(TagList tags)
        {
            return _tagManagerRepository.SaveTag(tags);
        }

        public int DeleteTag(TagList tags)
        {
            return _tagManagerRepository.DeleteTag(tags);
        }

        public int DeleteTags(TagList tags)
        {
            return _tagManagerRepository.DeleteTags(tags);
        }
    }
}