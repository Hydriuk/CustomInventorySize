using CustomInventorySize.Models;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomInventorySize.Services
{
    public class InventoryModifier
    {
        private readonly Dictionary<string, GroupSizes> _groupSizesProvider;

        public InventoryModifier(Configuration configuration)
        {
            _groupSizesProvider = configuration.Groups.ToDictionary(group => group.GroupName);
        }

        public void ModifyInventory(CSteamID playerId)
        {
            // Get the player's groups and order them by priority
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerId);
            var playerGroups = R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(p => p.Priority);

            // byte used to keep knowledge of the player's inventory pages that have been modified
            // We only keep to 0 pages that correspond to a player's inventory slot
            // Pages are : 
            // 0: Primary weapon slot
            // 1: Secondary weapon slot
            // 2: Player's hands
            // 3: Player's backpack
            // 4: Player's vest
            // 5: Player's shirt
            // 6: Player's pants
            // 7: Opened storage
            // 8: Nearby items
            // byte index goes from right to left
            byte modifiedPages = 0b_1000_0011;

            foreach (var playerGroup in playerGroups)
            {
                // Look for groups that are configured in the plugin
                if (!_groupSizesProvider.TryGetValue(playerGroup.Id, out GroupSizes groupSizes))
                    continue;

                byte pagesToModify = (byte)~modifiedPages;

                modifiedPages |= UpdatePages(uPlayer.Player, groupSizes, pagesToModify);

                // Stop if all pages are set
                if (modifiedPages == 0xFF)
                    break;
            }
        }

        public void ModifyPage(Player player, byte page)
        {
            // Get the player's groups and order them by priority
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
            var playerGroups = R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(p => p.Priority);

            foreach (var playerGroup in playerGroups)
            {
                // Look for groups that are configured in the plugin
                if (!_groupSizesProvider.TryGetValue(playerGroup.Id, out var groupSizes))
                    continue;

                byte modifiedPage = TryModifyPage(player, groupSizes, page);

                // Stop if the page has been modified
                if (modifiedPage != 0)
                    return;
            }
        }

        private byte TryModifyPage(Player player, GroupSizes sizes, byte pageIndex)
        {
            // Get the item id of the player's equipped item on the current slot
            ushort itemId = 0;
            if (pageIndex == PlayerInventory.BACKPACK)
                itemId = player.clothing.backpack;
            else if (pageIndex == PlayerInventory.VEST)
                itemId = player.clothing.vest;
            else if (pageIndex == PlayerInventory.SHIRT)
                itemId = player.clothing.shirt;
            else if (pageIndex == PlayerInventory.PANTS)
                itemId = player.clothing.pants;

            ItemStorageSize itemSize = null;
            if (itemId != 0)
                itemSize = sizes.Items.FirstOrDefault(item => item.ItemId == itemId);

            if (itemSize != null)
            {
                return SendModifyPage(player, pageIndex, itemSize.Width, itemSize.Height);
            }
            else
            {
                // If the size is not changed by the item
                PageSize pageSize = sizes.Pages.FirstOrDefault(page => page.PageIndex == pageIndex);

                if (pageSize != null)
                    return SendModifyPage(player, pageIndex, pageSize.Width, pageSize.Height);
            }

            return 0;
        }



        private byte UpdatePages(Player player, GroupSizes sizes, byte pagesToModify)
        {
            byte modifiedPages = 0;

            for (byte pageIndex = 0; pageIndex < PlayerInventory.PAGES - 2; pageIndex++)
            {
                // Check if current slot should be modified
                if (((byte)Math.Pow(2, pageIndex) & pagesToModify) == 0)
                    continue;

                modifiedPages |= TryModifyPage(player, sizes, pageIndex);
            }
            return modifiedPages;
        }


        private byte SendModifyPage(Player player, byte page, byte width, byte height)
        {
            // Update inventory size server side
            player.inventory.items[page].resize(width, height);

            // Drop items that exceed the new inventory space
            foreach (var item in player.inventory.items[page].items)
            {
                if (item.x + item.size_x > width || item.y + item.size_y > height)
                    player.inventory.sendDropItem(page, item.x, item.y);
            }

            // Convert the page index to its base 2 value
            return (byte)Math.Pow(2, page);
        }
    }
}
