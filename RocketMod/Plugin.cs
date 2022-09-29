using CustomInventorySize.API;
using CustomInventorySize.RocketMod.Events;
using CustomInventorySize.RocketMod.Services;
using CustomInventorySize.Services;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace CustomInventorySize.RocketMod
{
    public class Plugin : RocketPlugin<RocketConfiguration>
    {
        public static Plugin Instance { get; private set; }

        public IInventoryModifier InventoryModifier;
        private ISizesProvider _sizesProvider;

        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerClothingEquippedEvent _playerClothingEquippedEvent;
        private PlayerLifeUpdatedEvent _playerLifeUpdatedEvent;

        protected override void Load()
        {
            Instance = this;

            _sizesProvider = new SizesProvider(Configuration.Instance);
            InventoryModifier = new InventoryModifier(_sizesProvider);

            if (Configuration.Instance.Enabled)
            {
                _playerConnectedEvent = new PlayerConnectedEvent(InventoryModifier);
                _playerClothingEquippedEvent = new PlayerClothingEquippedEvent(InventoryModifier);
                _playerLifeUpdatedEvent = new PlayerLifeUpdatedEvent(InventoryModifier);
            }

            // Set the inventory size for all connected players
            foreach (var sPlayer in Provider.clients)
            {
                if (Configuration.Instance.Enabled)
                    InventoryModifier.ModifyInventory(sPlayer.playerID.steamID);
                else
                    InventoryModifier.ResetInventorySize(sPlayer.player);
            }
        }

        protected override void Unload()
        {
            if (Configuration.Instance.Enabled)
            {
                _playerConnectedEvent.Dispose();
                _playerClothingEquippedEvent.Dispose();
                _playerLifeUpdatedEvent.Dispose();
            }
        }
    }
}