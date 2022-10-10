using CustomInventorySize.API;
using CustomInventorySize.RocketMod.Events;
using CustomInventorySize.RocketMod.Services;
using CustomInventorySize.Services;
using HarmonyLib;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace CustomInventorySize.RocketMod
{
    public class Plugin : RocketPlugin<RocketConfiguration>
    {
        public static Plugin Instance { get; private set; }

        public IInventoryModifier InventoryModifier;
        private ISizesProvider _sizesProvider;
        private IThreadManager _threadManager;

        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerClothingEquippedEvent _playerClothingEquippedEvent;
        private PlayerLifeUpdatedEvent _playerLifeUpdatedEvent;
        private PlayerOpenedStorageEvent _playerOpenedStorageEvent;

        private Harmony _harmony;

        protected override void Load()
        {
            Instance = this;

            _sizesProvider = new SizesProvider(Configuration.Instance);
            _threadManager = new ThreadManager();
            InventoryModifier = new InventoryModifier(_sizesProvider, _threadManager);

            _playerConnectedEvent = new PlayerConnectedEvent(InventoryModifier);
            _playerClothingEquippedEvent = new PlayerClothingEquippedEvent(InventoryModifier);
            _playerLifeUpdatedEvent = new PlayerLifeUpdatedEvent(InventoryModifier);
            _playerOpenedStorageEvent = new PlayerOpenedStorageEvent(InventoryModifier);

            _harmony = new Harmony("Hydriuk.CustomInventorySize");
            _harmony.PatchAll();

            // Set the inventory size for all connected players
            foreach (var sPlayer in Provider.clients)
            {
                if (Configuration.Instance.Enabled)
                    InventoryModifier.ModifyInventory(sPlayer.player);
                else
                    InventoryModifier.ResetInventorySize(sPlayer.player);
            }
        }

        protected override void Unload()
        {
            _playerConnectedEvent.Dispose();
            _playerClothingEquippedEvent.Dispose();
            _playerLifeUpdatedEvent.Dispose();
            _playerOpenedStorageEvent.Dispose();
        }
    }
}