using CustomInventorySize.API;
using CustomInventorySize.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using Steamworks;
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
        private readonly IThreadManager _threadManager;

        public InventoryModifier(ISizesProvider sizesProvider, IThreadManager threadManager)
        {
            _sizesProvider = sizesProvider;
            _threadManager = threadManager;
        }

        public async void ModifyInventory(Player player)
        {
            // Get the player's groups ordered by priority
            List<GroupSizes> groupSizesList = await _sizesProvider.GetPrioritizedSizesAsync(player);

            EPage modifiedPages = EPage.None;

            // Preset empty clothes / storage
            if (player.clothing.backpack == 0) modifiedPages |= EPage.Backpack;
            if (player.clothing.vest == 0) modifiedPages |= EPage.Vest;
            if (player.clothing.shirt == 0) modifiedPages |= EPage.Shirt;
            if (player.clothing.pants == 0) modifiedPages |= EPage.Pants;
            if (!player.inventory.isStoring) modifiedPages |= EPage.Storage;

            foreach (var groupSizes in groupSizesList)
            {
                // Get inventory pages that need to be modified
                EPage pagesToModify = modifiedPages ^ (EPage.Inventory | EPage.Storage);

                // Update all pages with the current group configuration
                modifiedPages |= UpdatePages(player, groupSizes, pagesToModify);

                // Stop if all pages are set
                if (modifiedPages == (EPage.Inventory | EPage.Storage))
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

            if (pageIndex == PlayerInventory.STORAGE)
                ResetStorage(player);
        }

        public EPage SendModifyPage(Player player, byte pageIndex, byte width, byte height)
        {
            // Update inventory size
            player.inventory.items[pageIndex].resize(width, height);

            // Drop the items that are out of bounds of the page size
            DropExcess(player, pageIndex, width, height);

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
            else if (pageIndex == PlayerInventory.STORAGE)
            {
                BarricadeDrop barricade = BarricadeManager.FindBarricadeByRootTransform(player.inventory.storage.transform);
                itemId = barricade.asset.id;
            }

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

            return EPage.None;
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
            for (byte pageIndex = PlayerInventory.SLOTS; pageIndex < PlayerInventory.PAGES - 1; pageIndex++)
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
        private void DropExcess(Player player, byte pageIndex, byte width, byte height)
        {
            if (pageIndex == PlayerInventory.STORAGE)
                DropExcessStorage(player, width, height);
            else
                DropExcessInventory(player, pageIndex, width, height);
        }

        private void DropExcessInventory(Player player, byte pageIndex, byte width, byte height)
        {
            // Drop items that exceed the inventory space
            foreach (var item in player.inventory.items[pageIndex].items)
            {
                if (item.x + item.size_x > width || item.y + item.size_y > height)
                    player.inventory.sendDropItem(pageIndex, item.x, item.y);
            }
        }

        private void DropExcessStorage(Player player, byte width, byte height)
        {
            List<ItemJar> items = new List<ItemJar>();
            // Drop items that exceed the inventory space
            foreach (var item in player.inventory.storage.items.items)
            {
                if (!(item.x + item.size_x > width || item.y + item.size_y > height))
                    continue;

                // Drop item on floor
                ItemManager.dropItem(item.item, player.transform.position + player.transform.forward * 0.5f, true, true, false);

                items.Add(item);
            }

            // Remove dropped items from the storage
            _threadManager.RunOnMainThread(() =>
            {
                foreach (var item in items)
                {
                    byte index = player.inventory.getIndex(PlayerInventory.STORAGE, item.x, item.y);

                    player.inventory.removeItem(PlayerInventory.STORAGE, index);
                }
            });
        }

        /// <summary>
        /// Reset all player inventory pages to their original game size
        /// </summary>
        /// <param name="player"> Player of whom to reset the inventory </param>
        public void ResetInventorySize(Player player)
        {
            // Reset Hands
            ResetHands(player);

            // Reset backpack
            if (player.clothing.backpack != 0)
                ResetBackpack(player);

            // Reset vest
            if (player.clothing.vest != 0)
                ResetVest(player);

            // Reset shirt
            if (player.clothing.shirt != 0)
                ResetShirt(player);

            // Reset pants
            if (player.clothing.pants != 0)
                ResetPants(player);

            // Reset opened storage
            if (player.inventory.isStoring)
                ResetStorage(player);
        }

        public void ResetStorage(Player player)
        {
            BarricadeDrop barricade = BarricadeManager.FindBarricadeByRootTransform(player.inventory.storage.transform);
            ItemStorageAsset asset = (ItemStorageAsset)barricade.asset;

            DropExcessStorage(player, asset.storage_x, asset.storage_y);

            _threadManager.RunOnMainThread(() => player.inventory.items[PlayerInventory.STORAGE].resize(asset.storage_x, asset.storage_y));
        }

        private void ResetHands(Player player)
        {
            DropExcessInventory(player, PlayerInventory.SLOTS, 5, 3);
            player.inventory.items[PlayerInventory.SLOTS].resize(5, 3);
        }

        private void ResetBackpack(Player player)
        {
            byte width = player.clothing.backpackAsset.width;
            byte height = player.clothing.backpackAsset.height;

            DropExcessInventory(player, PlayerInventory.BACKPACK, width, height);
            player.inventory.items[PlayerInventory.BACKPACK].resize(player.clothing.backpackAsset.width, player.clothing.backpackAsset.height);
        }

        private void ResetVest(Player player)
        {
            byte width = player.clothing.vestAsset.width;
            byte height = player.clothing.vestAsset.height;

            DropExcessInventory(player, PlayerInventory.VEST, width, height);
            player.inventory.items[PlayerInventory.VEST].resize(player.clothing.vestAsset.width, player.clothing.vestAsset.height);
        }

        private void ResetShirt(Player player)
        {
            byte width = player.clothing.shirtAsset.width;
            byte height = player.clothing.shirtAsset.height;

            DropExcessInventory(player, PlayerInventory.SHIRT, width, height);
            player.inventory.items[PlayerInventory.SHIRT].resize(player.clothing.shirtAsset.width, player.clothing.shirtAsset.height);
        }

        private void ResetPants(Player player)
        {
            byte width = player.clothing.pantsAsset.width;
            byte height = player.clothing.pantsAsset.height;

            DropExcessInventory(player, PlayerInventory.PANTS, width, height);
            player.inventory.items[PlayerInventory.PANTS].resize(player.clothing.pantsAsset.width, player.clothing.pantsAsset.height);
        }
    }
}