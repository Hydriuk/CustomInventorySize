using CustomInventorySize.API;
using SDG.Unturned;
using System;

namespace CustomInventorySize.RocketMod.Events
{
    internal class PlayerLifeUpdatedEvent : IDisposable
    {
        private readonly IInventoryModifier _inventoryModifier;

        public PlayerLifeUpdatedEvent(IInventoryModifier inventoryModifier)
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