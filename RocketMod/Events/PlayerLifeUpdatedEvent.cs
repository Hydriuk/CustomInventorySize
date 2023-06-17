using CustomInventorySize.API;
using SDG.Unturned;
using System;

namespace CustomInventorySize.RocketMod.Events
{
    internal class PlayerLifeUpdatedEvent : IDisposable
    {
        private readonly IInventoryModifier _inventoryModifier;

        private readonly bool _enabled = Plugin.Instance.Configuration.Instance.Enabled;

        public PlayerLifeUpdatedEvent(IInventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;

            PlayerLife.onPlayerLifeUpdated += OnPlayerLifeUpdated;
        }

        public void Dispose()
        {
            PlayerLife.onPlayerLifeUpdated -= OnPlayerLifeUpdated;
        }

        private void OnPlayerLifeUpdated(Player player)
        {
            if (_enabled)
                _inventoryModifier.ModifyPage(player, PlayerInventory.SLOTS);
        }
    }
}