using CustomInventorySize.Services;
using SDG.Unturned;
using Steamworks;
using System;

namespace CustomInventorySize.Events
{
    public class PlayerConnectedEvent : IDisposable
    {
        private readonly InventoryModifier _inventoryModifier;

        public PlayerConnectedEvent(InventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;

            Provider.onServerConnected += OnPlayerConnected;
        }

        public void Dispose()
        {
            Provider.onServerConnected -= OnPlayerConnected;
        }

        private void OnPlayerConnected(CSteamID playerId) => _inventoryModifier.ModifyInventory(playerId);
    }
}