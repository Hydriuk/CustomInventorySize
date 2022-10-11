using CustomInventorySize.API;
using CustomInventorySize.Models;
using Cysharp.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Inventory.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod.Events
{
    public class PlayerOpenedStorageEvent : IEventListener<UnturnedPlayerOpenedStorageEvent>
    {
        private readonly IInventoryModifier _inventoryModifier;

        public PlayerOpenedStorageEvent(IInventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerOpenedStorageEvent @event)
        {
            if (Plugin.Enabled)
                _inventoryModifier.ModifyPageByRoles(@event.Player.Player, PlayerInventory.STORAGE);
            else
                _ = ResetOnMainThread(@event.Player.Player);

            return Task.CompletedTask;
        }

        private async UniTask ResetOnMainThread(Player player)
        {
            await UniTask.SwitchToMainThread();

            _inventoryModifier.ResetStorage(player);
        }
    }
}
