using CustomInventorySize.Models;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomInventorySize.Services
{
    public class InventoryModifier
    {
        private readonly Dictionary<string, GroupSizes> _groupSizesProvider;

        public InventoryModifier(Configuration configuration)
        {
            _groupSizesProvider = configuration.Groups.ToDictionary(group => group.GroupName);
        }

        /// <summary>
        /// Change the size of all player inventory pages to the one configured in their most prioritized group
        /// </summary>
        /// <param name="playerId"> Id of the player of whom to change the inventory size </param>
        public void ModifyInventory(CSteamID playerId)
        {
            // Get the player's groups and order them by priority
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerId);
            var playerGroups = R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(p => p.Priority);

            // byte used to keep in memory the player's inventory pages that have been modified
            // We only keep the pages that correspond to a player's inventory slot
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
                // Pass if the group is not configured
                if (!_groupSizesProvider.TryGetValue(playerGroup.Id, out GroupSizes groupSizes))
                    continue;

                // Bitwise complement operator to get a byte representing the pages to modify
                byte pagesToModify = (byte)~modifiedPages;

                // Update all pages with the current group configuration
                modifiedPages |= UpdatePages(uPlayer.Player, groupSizes, pagesToModify);

                // Stop if all pages are set
                if (modifiedPages == 0xFF)
                    break;
            }
        }

        /// <summary>
        /// Change the size of a single player inventory page to the one configured in their most prioritized group
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="page"> Page to change the size of </param>
        public void ModifyPage(Player player, byte page)
        {
            // Get the player's groups and order them by priority
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
            var playerGroups = R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(p => p.Priority);

            foreach (var playerGroup in playerGroups)
            {
                // Pass if the group is not configured
                if (!_groupSizesProvider.TryGetValue(playerGroup.Id, out var groupSizes))
                    continue;

                // Modifies the pages and returns the modified pages
                byte modifiedPage = TryModifyPage(player, groupSizes, page);

                // Stop if the page has been modified
                if (modifiedPage != 0)
                    return;
            }
        }

        /// <summary>
        /// Tries to change the size of a page
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="sizes"> Group configuration to use to change the page size </param>
        /// <param name="pageIndex"> Page to change the size of </param>
        /// <returns> A byte representing the indexes of the pages that have been changed </returns>
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

            // Get the configuration for this item
            ItemStorageSize itemSize = null;
            if (itemId != 0)
                itemSize = sizes.Items.FirstOrDefault(item => item.ItemId == itemId);

            if (itemSize != null)
            {
                // Change the page size
                return SendModifyPage(player, pageIndex, itemSize.Width, itemSize.Height);
            }
            else
            {
                // Get the configuration for this page
                PageSize pageSize = sizes.Pages.FirstOrDefault(page => page.PageIndex == pageIndex);

                // Change the page size
                if (pageSize != null)
                    return SendModifyPage(player, pageIndex, pageSize.Width, pageSize.Height);
            }

            return 0;
        }

        /// <summary>
        /// Change the size of all inventory pages of a player
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="sizes"> Group configuration to use to change the page size </param>
        /// <param name="pagesToModify"> A byte representing the indexes of the pages that need to be changed </param>
        /// <returns> A byte representing the indexes of the pages that have been changed </returns>
        private byte UpdatePages(Player player, GroupSizes sizes, byte pagesToModify)
        {
            byte modifiedPages = 0;

            // Loop through player inventory slots only
            for (byte pageIndex = PlayerInventory.SLOTS; pageIndex < PlayerInventory.PAGES - 2; pageIndex++)
            {
                // Pass if current page should not be modified
                if (((byte)Math.Pow(2, pageIndex) & pagesToModify) == 0)
                    continue;

                // Change the page size and update the modifiedPages byte
                modifiedPages |= TryModifyPage(player, sizes, pageIndex);
            }
            return modifiedPages;
        }

        /// <summary>
        /// Change the size of a page
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="pageIndex"> Page to change the size of </param>
        /// <param name="width"> New width of the page </param>
        /// <param name="height"> New height of the page </param>
        /// <returns> A byte representing the index of the page that has been changed </returns>
        private byte SendModifyPage(Player player, byte pageIndex, byte width, byte height)
        {
            // Update inventory size
            player.inventory.items[pageIndex].resize(width, height);

            // Drop the items that are out of bounds of the page size
            DropExcess(player, pageIndex);

            // Convert the page index to its base 2 value
            return (byte)Math.Pow(2, pageIndex);
        }

        /// <summary>
        /// Drop the items that are out of bounds of the page size
        /// </summary>
        /// <param name="player"> Player of whom to drop the excess items of </param>
        /// <param name="pageIndex"> Page of which to drop the items from </param>
        private void DropExcess(Player player, byte pageIndex)
        {
            byte width = player.inventory.items[pageIndex].width;
            byte height = player.inventory.items[pageIndex].height;

            // Drop items that exceed the inventory space
            foreach (var item in player.inventory.items[pageIndex].items)
            {
                if (item.x + item.size_x > width || item.y + item.size_y > height)
                    player.inventory.sendDropItem(pageIndex, item.x, item.y);
            }
        }

        /// <summary>
        /// Reset all player inventory pages to their original game size
        /// </summary>
        /// <param name="player"> Player of whom to reset the inventory </param>
        public void ResetInventorySize(Player player)
        {
            // Reset Hands
            player.inventory.items[PlayerInventory.SLOTS].resize(5, 3);
            DropExcess(player, 2);

            // Reset backpack
            if (player.clothing.backpack != 0)
            {
                player.inventory.items[PlayerInventory.BACKPACK].resize(player.clothing.backpackAsset.width, player.clothing.backpackAsset.height);
                DropExcess(player, PlayerInventory.BACKPACK);
            }

            // Reset vest
            if (player.clothing.vest != 0)
            {
                player.inventory.items[PlayerInventory.VEST].resize(player.clothing.vestAsset.width, player.clothing.vestAsset.height);
                DropExcess(player, PlayerInventory.VEST);
            }

            // Reset shirt
            if (player.clothing.shirt != 0)
            {
                player.inventory.items[PlayerInventory.SHIRT].resize(player.clothing.shirtAsset.width, player.clothing.shirtAsset.height);
                DropExcess(player, PlayerInventory.SHIRT);
            }

            // Reset pants
            if (player.clothing.pants != 0)
            {
                player.inventory.items[PlayerInventory.PANTS].resize(player.clothing.pantsAsset.width, player.clothing.pantsAsset.height);
                DropExcess(player, PlayerInventory.PANTS);
            }
        }
    }
}