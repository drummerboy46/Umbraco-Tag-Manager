using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Our.Umbraco.Community.TagManager.Models
{
    [DataContract(Name = "pagedContent", Namespace = "")]
    public class PagedContent
    {
        [DataMember(Name = "taggedContent")]
        public List<TaggedContent> TaggedContent { get; set; }

        [DataMember(Name = "totalRecords")]
        public int TotalRecords { get; set; }
    }
}