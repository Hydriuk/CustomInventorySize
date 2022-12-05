using CustomInventorySize.API;
using CustomInventorySize.RocketMod.Events;
using CustomInventorySize.RocketMod.Services;
using CustomInventorySize.Services;
using HarmonyLib;
using PermissionsModule.API;
using PermissionsModule.RocketMod;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace CustomInventorySize.RocketMod
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance { get; private set; }

        private ISizesProvider _sizesProvider;
        private ITranslationsAdapter _translationsAdapter;
        private IPermissionsAdapter _permissionsAdapter;
        private IThreadAdapter _threadAdapter;
        private IInventoryModifier _inventoryModifier;
        private IChatMessenger _chatMessenger;

        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerClothingEquippedEvent _playerClothingEquippedEvent;
        private PlayerLifeUpdatedEvent _playerLifeUpdatedEvent;
        private PlayerOpenedStorageEvent _playerOpenedStorageEvent;

        private Harmony _harmony;

        protected override void Load()
        {
            Instance = this;

            _permissionsAdapter = new PermissionsAdapter();
            _threadAdapter = new ThreadAdapter();
            _sizesProvider = new SizesProvider(Configuration.Instance, _permissionsAdapter);
            _translationsAdapter = new TranslationsAdapter(Translations.Instance);
            _chatMessenger = new ChatMessenger(_translationsAdapter, _threadAdapter);
            _inventoryModifier = new InventoryModifier(_sizesProvider, _threadAdapter, _chatMessenger);

            _playerConnectedEvent = new PlayerConnectedEvent(_inventoryModifier);
            _playerClothingEquippedEvent = new PlayerClothingEquippedEvent(_inventoryModifier);
            _playerLifeUpdatedEvent = new PlayerLifeUpdatedEvent(_inventoryModifier);
            _playerOpenedStorageEvent = new PlayerOpenedStorageEvent(_inventoryModifier);

            _harmony = new Harmony("Hydriuk.CustomInventorySize");
            _harmony.PatchAll();

            // Set the inventory size for all connected players
            foreach (var sPlayer in Provider.clients)
            {
                if (Configuration.Instance.Enabled)
                    _inventoryModifier.ModifyInventoryByRoles(sPlayer.player);
                else
                    _inventoryModifier.ResetInventorySize(sPlayer.player);
            }
        }

        protected override void Unload()
        {
            _playerConnectedEvent.Dispose();
            _playerClothingEquippedEvent.Dispose();
            _playerLifeUpdatedEvent.Dispose();
            _playerOpenedStorageEvent.Dispose();

            _harmony.UnpatchAll();
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