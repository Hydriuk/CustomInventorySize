using CustomInventorySize.API;
using CustomInventorySize.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomInventorySize.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class InventoryModifier : IInventoryModifier
    {
        private readonly ISizesProvider _sizesProvider;

        public InventoryModifier(ISizesProvider sizesProvider)
        {
            _sizesProvider = sizesProvider;
        }

        public void ModifyInventory(CSteamID playerId)
        {
            Player player = PlayerTool.getPlayer(playerId);

            ModifyInventory(player);
        }

        public async void ModifyInventory(Player player)
        {
            // Get the player's groups ordered by priority
            List<GroupSizes> groupSizesList = await _sizesProvider.GetPrioritizedSizesAsync(player);

            EPage modifiedPages = EPage.None;

            // Preset empty clothes
            if (player.clothing.backpack == 0) modifiedPages |= EPage.Backpack;
            if (player.clothing.vest     == 0) modifiedPages |= EPage.Vest;
            if (player.clothing.shirt    == 0) modifiedPages |= EPage.Shirt;
            if (player.clothing.pants    == 0) modifiedPages |= EPage.Pants;

            foreach (var groupSizes in groupSizesList)
            {
                // Get inventory pages that need to be modified
                EPage pagesToModify = modifiedPages ^ EPage.Inventory;

                // Update all pages with the current group configuration
                modifiedPages |= UpdatePages(player, groupSizes, pagesToModify);

                // Stop if all pages are set
                if (modifiedPages == EPage.Inventory)
                    break;
            }

            // Reset pages that were not modified
            if ((EPage.Hands ^ modifiedPages) == EPage.Hands)
                ResetHands(player);
            if ((EPage.Backpack ^ modifiedPages) == EPage.Backpack)
                ResetBackpack(player);
            if ((EPage.Vest ^ modifiedPages) == EPage.Vest)
                ResetVest(player);
            if ((EPage.Shirt ^ modifiedPages) == EPage.Shirt)
                ResetShirt(player);
            if ((EPage.Pants ^ modifiedPages) == EPage.Pants)
                ResetPants(player);
        }

        public async void ModifyPage(Player player, byte pageIndex)
        {
            // Get the player's groups ordered by priority
            List<GroupSizes> groupSizesList = await _sizesProvider.GetPrioritizedSizesAsync(player);

            foreach (var groupSizes in groupSizesList)
            {
                // Modifies the pages and returns the modified pages
                EPage modifiedPage = TryModifyPage(player, groupSizes, pageIndex);

                // Stop if the page has been modified
                if (modifiedPage != EPage.None)
                    return;
            }
        }

        public EPage SendModifyPage(Player player, byte pageIndex, byte width, byte height)
        {
            // Update inventory size
            player.inventory.items[pageIndex].resize(width, height);

            // Drop the items that are out of bounds of the page size
            DropExcess(player, pageIndex);

            // Convert the page index to EPage
            return (EPage)(1 << pageIndex);
        }

        /// <summary>
        /// Tries to change the size of a page
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="sizes"> Group configuration to use to change the page size </param>
        /// <param name="pageIndex"> Page to change the size of </param>
        /// <returns> A byte representing the indexes of the pages that have been changed </returns>
        private EPage TryModifyPage(Player player, GroupSizes sizes, byte pageIndex)
        {
            if (pageIndex == PlayerInventory.SLOTS)
            {
                // Get the configuration for this page
                PageSize pageSize = sizes.Pages.FirstOrDefault(page => page.Index == pageIndex);

                // Change the page size
                if (pageSize != null)
                    return SendModifyPage(player, pageIndex, pageSize.Width, pageSize.Height);

                return 0;
            }

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

            // The item was removed, ignore the page
            if (itemId == 0)
                return (EPage)(1 << pageIndex);

            // Get the configuration for this item
            ItemStorageSize itemSize = sizes.Items.FirstOrDefault(item => item.ItemId == itemId);

            if (itemSize != null)
            {
                // Change the page size
                return SendModifyPage(player, pageIndex, itemSize.Width, itemSize.Height);
            }
            else
            {
                // Get the default configuration for this page
                PageSize pageSize = sizes.Pages.FirstOrDefault(page => page.Index == pageIndex);

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
        private EPage UpdatePages(Player player, GroupSizes sizes, EPage pagesToModify)
        {
            EPage modifiedPages = 0;

            // Loop through player inventory slots only
            for (byte pageIndex = PlayerInventory.SLOTS; pageIndex < PlayerInventory.PAGES - 2; pageIndex++)
            {
                EPage currentPage = (EPage)(1 << pageIndex);

                // Pass if current page should not be modified
                // In other word, is currentPage in pagesToModify ?
                if ((currentPage & pagesToModify) != currentPage)
                    continue;

                // Change the page size and update the modifiedPages byte
                modifiedPages |= TryModifyPage(player, sizes, pageIndex);
            }

            return modifiedPages;
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
            ResetHands(player);
            DropExcess(player, 2);

            // Reset backpack
            if (player.clothing.backpack != 0)
            {
                ResetBackpack(player);
                DropExcess(player, PlayerInventory.BACKPACK);
            }

            // Reset vest
            if (player.clothing.vest != 0)
            {
                ResetVest(player);
                DropExcess(player, PlayerInventory.VEST);
            }

            // Reset shirt
            if (player.clothing.shirt != 0)
            {
                ResetShirt(player);
                DropExcess(player, PlayerInventory.SHIRT);
            }

            // Reset pants
            if (player.clothing.pants != 0)
            {
                ResetPants(player);
                DropExcess(player, PlayerInventory.PANTS);
            }
        }

        private void ResetHands(Player player) => player.inventory.items[PlayerInventory.SLOTS].resize(5, 3);
        private void ResetBackpack(Player player) => player.inventory.items[PlayerInventory.BACKPACK].resize(player.clothing.backpackAsset.width, player.clothing.backpackAsset.height);
        private void ResetVest(Player player) => player.inventory.items[PlayerInventory.VEST].resize(player.clothing.vestAsset.width, player.clothing.vestAsset.height);
        private void ResetShirt(Player player) => player.inventory.items[PlayerInventory.SHIRT].resize(player.clothing.shirtAsset.width, player.clothing.shirtAsset.height);
        private void ResetPants(Player player) => player.inventory.items[PlayerInventory.PANTS].resize(player.clothing.pantsAsset.width, player.clothing.pantsAsset.height);


    }
}