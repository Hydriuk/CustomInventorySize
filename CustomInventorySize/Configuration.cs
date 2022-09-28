using CustomInventorySize.Models;
using Rocket.API;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize
{
    public class Configuration : IRocketPluginConfiguration
    {
        public List<GroupSizes> Groups { get; set; }

        public void LoadDefaults()
        {
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
