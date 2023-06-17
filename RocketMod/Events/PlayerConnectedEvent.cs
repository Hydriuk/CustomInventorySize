using CustomInventorySize.API;
using SDG.Unturned;
using Steamworks;
using System;

namespace CustomInventorySize.RocketMod.Events
{
    public class PlayerConnectedEvent : IDisposable
    {
        private readonly IInventoryModifier _inventoryModifier;

        private readonly bool _enabled = Plugin.Instance.Configuration.Instance.Enabled;

        public PlayerConnectedEvent(IInventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;

            Provider.onServerConnected += OnPlayerConnected;
        }

        public void Dispose()
        {
            Provider.onServerConnected -= OnPlayerConnected;
        }

        private void OnPlayerConnected(CSteamID playerId)
        {
            Player player = PlayerTool.getPlayer(playerId);

            if (_enabled)
                _inventoryModifier.ModifyInventory(player);
            else
                _inventoryModifier.ResetInventorySize(player);
        }
    }
}