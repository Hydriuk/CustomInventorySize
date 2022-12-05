using CustomInventorySize.API;
using CustomInventorySize.Models;
using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;

namespace CustomInventorySize.RocketMod
{
    public class Configuration : IConfigurationAdapter, IRocketPluginConfiguration
    {
        public bool Enabled { get; set; }
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
                        new PageSize(PlayerInventory.SLOTS, 5, 5)
                    },
                    Items = new List<ItemStorageSize>()
                    {
                        new ItemStorageSize(253, 10, 20)
                    }
                }
            };
        }
    }
}