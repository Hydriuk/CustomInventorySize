using CustomInventorySize.API;
using CustomInventorySize.RocketMod.Events;
using CustomInventorySize.Services;
using HarmonyLib;
using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace CustomInventorySize.RocketMod
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance { get; private set; }

        private ISizesProvider _sizesProvider;
        private ITranslationAdapter _translationsAdapter;
        private IPermissionAdapter _permissionsAdapter;
        private IThreadAdapter _threadAdapter;
        private IInventoryModifier _inventoryModifier;
        private IChatMessenger _chatMessenger;
        private IStorageModifier _storageModifier;

        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerClothingEquippedEvent _playerClothingEquippedEvent;
        private PlayerLifeUpdatedEvent _playerLifeUpdatedEvent;
        private BarricadeDeployedEvent _barricadeDeployedEvent;

        private Harmony _harmony;

        protected override void Load()
        {
            Instance = this;

            _permissionsAdapter = new PermissionAdapter();
            _threadAdapter = new ThreadAdapter();
            _sizesProvider = new SizesProvider(_permissionsAdapter);
            _translationsAdapter = new TranslationAdapter(Translations.Instance);
            _chatMessenger = new ChatMessenger(_translationsAdapter, _threadAdapter);
            _inventoryModifier = new InventoryModifier(_sizesProvider, _threadAdapter, _chatMessenger);
            _storageModifier = new StorageModifier(_sizesProvider, _threadAdapter, _inventoryModifier);

            _playerConnectedEvent = new PlayerConnectedEvent(_inventoryModifier);
            _playerClothingEquippedEvent = new PlayerClothingEquippedEvent(_inventoryModifier);
            _playerLifeUpdatedEvent = new PlayerLifeUpdatedEvent(_inventoryModifier);
            _barricadeDeployedEvent = new BarricadeDeployedEvent(_storageModifier);

            _harmony = new Harmony("Hydriuk.CustomInventorySize");
            _harmony.PatchAll();

            // Set the inventory size for all connected players
            foreach (var sPlayer in Provider.clients)
            {
                if (Configuration.Instance.Enabled)
                    _inventoryModifier.ModifyInventory(sPlayer.player);
                else
                    _inventoryModifier.ResetInventorySize(sPlayer.player);
            }

            if (Level.isLoaded)
                LateLoad(0);
            else
                Level.onPostLevelLoaded += LateLoad;
        }

        protected override void Unload()
        {
            _playerConnectedEvent.Dispose();
            _playerClothingEquippedEvent.Dispose();
            _playerLifeUpdatedEvent.Dispose();
            _barricadeDeployedEvent.Dispose();

            _harmony.UnpatchAll("Hydriuk.CustomInventorySize");
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

                    if (Configuration.Instance.Enabled)
                        _storageModifier.ModifyStorage(storage, barricade.asset.id);
                    else
                        _storageModifier.ResetStorage(barricade);
                }
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "StorageItemDropped", "Items from the storage were dropped because it was resized" },
            { "InventoryItemDropped", "Items from your inventory were dropped because it was resized" }
        };

        public void ResetInventorySize(Player player) => _inventoryModifier.ResetInventorySize(player);

        public void SendModifyPage(Player player, byte pageIndex, byte width, byte height) => _inventoryModifier.ModifyPage(player, pageIndex, width, height);
    }
}