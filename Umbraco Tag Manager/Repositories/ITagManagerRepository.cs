using System.Collections.Generic;
using Umbraco.Community.TagManager.Models;

namespace Umbraco.Community.TagManager.Repositories
{
    public interface ITagManagerRepository
    {
        TagList GetTagsById(int id);

        TagList GetTagsByGroup(string group);

        List<TagGroup> GetTagGroups();

        PagedContent GetPagedContent(int id, int offset = 0, int limit = 10);

        PagedMedia GetPagedMedia(int id, int offset = 0, int limit = 10);

        int CreateGroup(string group = "default");

        int CreateTag(TagItem tag);

        int SaveTag(TagList tags);

        int DeleteTag(TagList tags);

        int DeleteTags(TagList tags);
    }
}
