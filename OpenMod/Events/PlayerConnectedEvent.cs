using CustomInventorySize.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Connections.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod.Events
{
    public class PlayerConnectedEvent : IEventListener<UnturnedPlayerConnectedEvent>
    {
        private readonly IInventoryModifier _inventoryModifier;

        public PlayerConnectedEvent(IInventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerConnectedEvent @event)
        {
            if (!Plugin.Enabled)
                return Task.CompletedTask;

            _inventoryModifier.ModifyInventory(@event.Player.Player);

            return Task.CompletedTask;
        }
    }
}
