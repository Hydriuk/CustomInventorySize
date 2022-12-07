using CustomInventorySize.API;
using Cysharp.Threading.Tasks;
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
        private readonly IStorageModifier _storageModifier;

        public Plugin(
            IServiceProvider serviceProvider,
            IConfigurationAdapter configuration,
            IInventoryModifier inventoryModifier,
            IStorageModifier storageModifier) : base(serviceProvider)
        {
            Enabled = configuration.Enabled;

            _inventoryModifier = inventoryModifier;
            _storageModifier = storageModifier;
        }

        protected override UniTask OnLoadAsync()
        {
            // Set the inventory size for all connected players
            foreach (var sPlayer in Provider.clients)
            {
                if (Enabled)
                    _inventoryModifier.ModifyInventoryByRoles(sPlayer.player);
                else
                    _inventoryModifier.ResetInventorySize(sPlayer.player);
            }

            if (Level.isLoaded)
                LateLoad(0);
            else
                Level.onPostLevelLoaded += LateLoad;

            return UniTask.CompletedTask;
        }

        private void LateLoad(int level)
        {
            Level.onPostLevelLoaded -= LateLoad;

            foreach (var barricadeRegion in BarricadeManager.BarricadeRegions)
            {
                if (barricadeRegion == null || barricadeRegion.drops == null)
                    continue;

                foreach (var barricade in barricadeRegion.drops)
                {
                    if (barricade.interactable is not InteractableStorage storage)
                        continue;

                    if (Enabled)
                        _storageModifier.ModifyStorage(storage, barricade.asset.id);
                    else
                        _storageModifier.ResetStorage(barricade);
                }
            }
        }
    }
}