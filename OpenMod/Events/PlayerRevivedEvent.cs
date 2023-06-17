using CustomInventorySize.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Life.Events;
using SDG.Unturned;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod.Events
{
    internal class PlayerRevivedEvent : IEventListener<UnturnedPlayerRevivedEvent>
    {
        private readonly IInventoryModifier _inventoryModifier;

        public PlayerRevivedEvent(IInventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerRevivedEvent @event)
        {
            if (!Plugin.Enabled)
                return Task.CompletedTask;

            _inventoryModifier.ModifyPage(@event.Player.Player, PlayerInventory.SLOTS);

            return Task.CompletedTask;
        }
    }
}