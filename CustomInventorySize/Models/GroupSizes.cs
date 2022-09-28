using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CustomInventorySize.Models
{
    public class GroupSizes
    {
        public string GroupName { get; set; } = string.Empty;

        [XmlArray]
        public List<ItemStorageSize> Items { get; set; } = new List<ItemStorageSize>();

        [XmlArray]
        public List<PageSize> Pages { get; set; } = new List<PageSize>();

        public GroupSizes() { }

        public GroupSizes(string groupName)
        {
            GroupName = groupName;
        }
    }
}
