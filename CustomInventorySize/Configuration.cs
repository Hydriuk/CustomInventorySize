using CustomInventorySize.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CustomInventorySize
{
    public class Configuration
    {
        public bool Enabled { get; set; }

        [XmlArrayItem("Group")]
        public List<GroupSizes> Groups { get; set; } = new List<GroupSizes>();
    }
}