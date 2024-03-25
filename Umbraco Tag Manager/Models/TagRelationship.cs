using System.Runtime.Serialization;

namespace Umbraco.Community.TagManager.Models
{
    [DataContract(Name = "tagRelationship", Namespace = "")]
    public class TagRelationship
    {
        [DataMember(Name = "nodeId")]
        public int NodeId { get; set; }

        [DataMember(Name = "tagId")]
        public int TagId { get; set; }

        [DataMember(Name = "propertyTypeId")]
        public int PropertyTypeId { get; set; }
    }
}