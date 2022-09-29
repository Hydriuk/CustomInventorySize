using System.Collections.Generic;
using System.Xml.Serialization;
using SDG.Unturned;

namespace CustomInventorySize.Models
{
    public class GroupSizes
    {
        [XmlAttribute()]
        public string GroupName { get; set; } = string.Empty;

        [XmlArrayItem("ItemStorage")]
        public List<ItemStorageSize> Items { get; set; } = new List<ItemStorageSize>();

        [XmlArrayItem("Page")]
        public List<PageSize> Pages { get; set; } = new List<PageSize>();

        public GroupSizes()
        { }

        public GroupSizes(string groupName)
        {
            GroupName = groupName;
        }
    }
}