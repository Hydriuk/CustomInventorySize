using CustomInventorySize.Services;
using SDG.Unturned;
using System;

namespace CustomInventorySize.Events
{
    internal class PlayerLifeUpdatedEvent : IDisposable
    {
        private readonly InventoryModifier _inventoryModifier;

        public PlayerLifeUpdatedEvent(InventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;

            PlayerLife.onPlayerLifeUpdated += OnPlayerLifeUpdated;
        }

        public void Dispose()
        {
            PlayerLife.onPlayerLifeUpdated -= OnPlayerLifeUpdated;
        }

        private void OnPlayerLifeUpdated(Player player) => _inventoryModifier.ModifyPage(player, PlayerInventory.SLOTS);
    }
}