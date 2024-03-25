using System.Runtime.Serialization;

namespace Umbraco.Community.TagManager.Models
{
    [DataContract(Name = "taggedContent", Namespace = "")]
    public class TaggedContent
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}