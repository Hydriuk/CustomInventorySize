using CustomInventorySize.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Unturned.Players.Clothing;
using OpenMod.Unturned.Players.Clothing.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

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

            switch (@event.Type)
            {
                case ClothingType.Backpack:
                    _inventoryModifier.ModifyPage(@event.Player.Player, PlayerInventory.BACKPACK);
                    break;

                case ClothingType.Vest:
                    _inventoryModifier.ModifyPage(@event.Player.Player, PlayerInventory.VEST);
                    break;

                case ClothingType.Shirt:
                    _inventoryModifier.ModifyPage(@event.Player.Player, PlayerInventory.SHIRT);
                    break;

                case ClothingType.Pants:
                    _inventoryModifier.ModifyPage(@event.Player.Player, PlayerInventory.PANTS);
                    break;
            }
         
            return Task.CompletedTask;
        }
    }
}
