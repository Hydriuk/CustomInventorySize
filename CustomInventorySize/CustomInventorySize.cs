using CustomInventorySize.Events;
using CustomInventorySize.Services;
using Rocket.Core.Plugins;
using SDG.Unturned;

namespace CustomInventorySize
{
    public class CustomInventorySize : RocketPlugin<Configuration>
    {
        public static CustomInventorySize Instance { get; private set; }

        public InventoryModifier InventoryModifier;

        private PlayerConnectedEvent _playerConnectedEvent;
        private PlayerClothingEquippedEvent _playerClothingEquippedEvent;
        private PlayerLifeUpdatedEvent _playerLifeUpdatedEvent;

        protected override void Load()
        {
            Instance = this;

            InventoryModifier = new InventoryModifier(Configuration.Instance);

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