using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Our.Umbraco.Community.TagManager.Models
{
    [DataContract(Name = "tagList", Namespace = "")]
    public class TagList
    {
        [DataMember(Name = "tagItem")]
        public TagItem TagItem { get; set; }

        [DataMember(Name = "tagsInGroup")]
        public List<TagItem> TagsInGroup { get; set; }

        [DataMember(Name = "mergeTag")]
        public TagItem MergeTag { get; set; }
    }
}