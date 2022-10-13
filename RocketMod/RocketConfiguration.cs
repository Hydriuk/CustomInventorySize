using CustomInventorySize.Models;
using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;

namespace CustomInventorySize.RocketMod
{
    public class RocketConfiguration : Configuration, IRocketPluginConfiguration
    {
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
                        new ItemStorageSize(253, 10, 12)
                    }
                }
            };
        }
    }
}