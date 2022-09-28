using CustomInventorySize.Models;
using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CustomInventorySize
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled { get; set; }

        [XmlArrayItem("Group")]
        public List<GroupSizes> Groups { get; set; }

        public void LoadDefaults()
        {
            Enabled = true;
            Groups = new List<GroupSizes>()
            {
                new GroupSizes("default")
                {
                    Pages = new List<PageSize>()
                    {
                        new PageSize(PlayerInventory.SLOTS, 1, 1)
                    },
                    Items = new List<ItemStorageSize>()
                    {
                        new ItemStorageSize(253, 20, 40)
                    }
                }
            };
        }
    }
}