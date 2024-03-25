using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Umbraco.Community.TagManager.Models
{
    [DataContract(Name = "pagedMedia", Namespace = "")]
    public class PagedMedia
    {
        [DataMember(Name = "taggedMedia")]
        public List<TaggedMedia> TaggedMedia { get; set; }

        [DataMember(Name = "totalRecords")]
        public int TotalRecords { get; set; }
    }
}