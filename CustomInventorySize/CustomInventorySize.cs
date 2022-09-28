using CustomInventorySize.Events;
using CustomInventorySize.Services;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace CustomInventorySize
{
    public class CustomInventorySize : RocketPlugin<Configuration>
    {
        public static CustomInventorySize Instance { get; private set; }

        private InventoryModifier _inventoryModifier;

        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerClothingEquippedEvent _playerClothingEquippedEvent;
        private PlayerLifeUpdatedEvent _playerLifeUpdatedEvent;

        protected override void Load()
        {
            Instance = this;

            _inventoryModifier = new InventoryModifier(Configuration.Instance);

            if (Configuration.Instance.Enabled)
            {
                _playerConnectedEvent = new PlayerConnectedEvent(_inventoryModifier);
                _playerClothingEquippedEvent = new PlayerClothingEquippedEvent(_inventoryModifier);
                _playerLifeUpdatedEvent = new PlayerLifeUpdatedEvent(_inventoryModifier);
            }

            // Set the inventory size for all connected players
            foreach (var sPlayer in Provider.clients)
            {
                if (Configuration.Instance.Enabled)
                    _inventoryModifier.ModifyInventory(sPlayer.playerID.steamID);
                else
                    _inventoryModifier.ResetInventorySize(sPlayer.player);
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