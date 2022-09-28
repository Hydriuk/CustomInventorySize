using CustomInventorySize.Models;
using CustomInventorySize.Services;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rocket.Unturned.Events.UnturnedEvents;

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
