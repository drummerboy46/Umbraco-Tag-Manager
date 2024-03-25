using System.Runtime.Serialization;

namespace Our.Umbraco.Community.TagManager.Models
{
    [DataContract(Name = "tagGroup", Namespace = "")]
    public class TagGroup
    {
        [DataMember(Name = "group")]
        public string Group { get; set; }
    }
}