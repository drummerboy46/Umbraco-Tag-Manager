using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Our.Umbraco.Community.TagManager.Models
{
    [DataContract(Name = "tagItem", Namespace = "")]
    public class TagItem
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "tag")]
        public string Tag { get; set; }

        [DataMember(Name = "group")]
        public string Group { get; set; }

        [DataMember(Name = "tagSelected")]
        public bool TagSelected { get; set; }

        [DataMember(Name = "tagRelationshipCount")]
        public int TagRelationshipCount { get; set; }

        [DataMember(Name = "tagRelationships")]
        public List<TagRelationship> TagRelationships { get; set; }

        [DataMember(Name = "taggedContent")]
        public List<TaggedContent> TaggedContent { get; set; }

        [DataMember(Name = "taggedMedia")]
        public List<TaggedMedia> TaggedMedia { get; set; }
    }
}