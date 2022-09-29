using CustomInventorySize.API;
using SDG.Unturned;
using Steamworks;
using System;

namespace CustomInventorySize.RocketMod.Events
{
    public class PlayerConnectedEvent : IDisposable
    {
        private readonly IInventoryModifier _inventoryModifier;

        public PlayerConnectedEvent(IInventoryModifier inventoryModifier)
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