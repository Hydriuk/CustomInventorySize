using HarmonyLib;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.Events
{
    [HarmonyPatch(typeof(PlayerInventory), "loadSize")]
    internal class ItemDiscard
    {
        public static event Func<Player, byte, ushort, bool>? ItemDiscarding;

        [HarmonyPatch("onShirtUpdated")]
        [HarmonyPrefix()]
        private static bool onShirtUpdated(PlayerInventory __instance, ushort id)
        {
            return ItemDiscarding?.Invoke(__instance.player, PlayerInventory.SHIRT, id) ?? true;
        }

        [HarmonyPatch("onPantsUpdated")]
        [HarmonyPrefix()]
        private static bool onPantsUpdated(PlayerInventory __instance, ushort id)
        {
            return ItemDiscarding?.Invoke(__instance.player, PlayerInventory.PANTS, id) ?? true;
        }

        [HarmonyPatch("onBackpackUpdated")]
        [HarmonyPrefix()]
        private static bool onBackpackUpdated(PlayerInventory __instance, ushort id)
        {
            return ItemDiscarding?.Invoke(__instance.player, PlayerInventory.BACKPACK, id) ?? true;
        }

        [HarmonyPatch("onVestUpdated")]
        [HarmonyPrefix()]
        private static bool onVestUpdated(PlayerInventory __instance, ushort id)
        {
            return ItemDiscarding?.Invoke(__instance.player, PlayerInventory.VEST, id) ?? true;
        }
    }
}
