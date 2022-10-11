using CustomInventorySize.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Clothing;
using OpenMod.Unturned.Players.Clothing.Events;
using SDG.Unturned;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod.Events
{
    public class PlayerClothingEquippedEvent : IEventListener<UnturnedPlayerClothingEquippedEvent>
    {
        private readonly IInventoryModifier _inventoryModifier;

        public PlayerClothingEquippedEvent(IInventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerClothingEquippedEvent @event)
        {
            if (!Plugin.Enabled)
                return Task.CompletedTask;

            // We run the logic in a task to let the handler finish. As long as the handler hasn't finished, the item is not equipped.
            // Thus, the size is changed before and then, when equipped, the item changes the size back to normal.
            Task.Run(() =>
            {
                switch (@event.Type)
                {
                    case ClothingType.Backpack:
                        _inventoryModifier.ModifyPageByRoles(@event.Player.Player, PlayerInventory.BACKPACK);
                        break;

                    case ClothingType.Vest:
                        _inventoryModifier.ModifyPageByRoles(@event.Player.Player, PlayerInventory.VEST);
                        break;

                    case ClothingType.Shirt:
                        _inventoryModifier.ModifyPageByRoles(@event.Player.Player, PlayerInventory.SHIRT);
                        break;

                    case ClothingType.Pants:
                        _inventoryModifier.ModifyPageByRoles(@event.Player.Player, PlayerInventory.PANTS);
                        break;
                }
            });

            return Task.CompletedTask;
        }
    }
}