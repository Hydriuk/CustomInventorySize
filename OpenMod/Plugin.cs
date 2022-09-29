using CustomInventorySize.API;
using CustomInventorySize.Services;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using SDG.Unturned;
using System;

[assembly: PluginMetadata("CustomInventorySize", DisplayName = "CustomInventorySize", Author = "Hydriuk")]

namespace CustomInventorySize.OpenMod
{
    public class Plugin : OpenModUnturnedPlugin
    {
        public static bool Enabled = false;

        private readonly IInventoryModifier _inventoryModifier;

        public Plugin(
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IInventoryModifier inventoryModifier) : base(serviceProvider)
        {
            Enabled = configuration.GetValue<bool>("Enabled");

            _inventoryModifier = inventoryModifier;
        }

        protected override UniTask OnLoadAsync()
        {
            // Set the inventory size for all connected players
            foreach (var sPlayer in Provider.clients)
            {
                if (Enabled)
                    _inventoryModifier.ModifyInventory(sPlayer.playerID.steamID);
                else
                    _inventoryModifier.ResetInventorySize(sPlayer.player);
            }

            return UniTask.CompletedTask;
        }
    }
}