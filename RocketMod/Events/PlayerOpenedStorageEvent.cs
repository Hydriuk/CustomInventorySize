using CustomInventorySize.API;
using HarmonyLib;
using Rocket.Core.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.RocketMod.Events
{
    public class PlayerOpenedStorageEvent : IDisposable
    {
        private readonly IInventoryModifier _inventoryModifier;

        private readonly bool _enabled = Plugin.Instance.Configuration.Instance.Enabled;

        public PlayerOpenedStorageEvent(IInventoryModifier inventoryModifier)
        {
            _inventoryModifier = inventoryModifier;

            OnOpenedStorage += OnStorageOpened;
        }

        public void Dispose()
        {
            OnOpenedStorage -= OnStorageOpened;
        }

        private void OnStorageOpened(Player player)
        {
            if (_enabled)
                _inventoryModifier.ModifyPage(player, PlayerInventory.STORAGE);
            else
                TaskDispatcher.QueueOnMainThread(() => _inventoryModifier.ResetStorage(player));
        }

        private delegate void OpenedStorage(Player player);
        private static event OpenedStorage OnOpenedStorage;

        [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.openStorage))]
        public class OpenStoragePatch
        {
            public static void Postfix(PlayerInventory __instance)
            {
                OnOpenedStorage?.Invoke(__instance.player);
            }
        }
    }
}
