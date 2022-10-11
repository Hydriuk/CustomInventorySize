using CustomInventorySize.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Connections.Events;
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
            if (Plugin.Enabled)
                _inventoryModifier.ModifyInventoryByRoles(@event.Player.Player);
            else
                _inventoryModifier.ResetInventorySize(@event.Player.Player);

            return Task.CompletedTask;
        }
    }
}