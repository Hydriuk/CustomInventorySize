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
using static SDG.Provider.SteamGetInventoryResponse;

namespace CustomInventorySize.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class InventoryModifier : IInventoryModifier
    {
        private readonly ISizesProvider _sizesProvider;
        private readonly IThreadAdapter _threads;
        private readonly IChatMessenger _chatMessenger;

        public InventoryModifier(ISizesProvider sizesProvider, IThreadAdapter threads, IChatMessenger chatMessenger)
        {
            _sizesProvider = sizesProvider;
            _threads = threads;
            _chatMessenger = chatMessenger;
        }

        public async void ModifyInventoryByRoles(Player player)
        {
            // Get the player's groups ordered by priority
            List<GroupSizes> groupSizesList = await _sizesProvider.GetPrioritizedSizesAsync(player.channel.owner.playerID.steamID);

            EPage modifiedPages = EPage.Storage;

            // Preset empty clothes / storage
            if (player.clothing.backpack == 0) modifiedPages |= EPage.Backpack;
            if (player.clothing.vest == 0) modifiedPages |= EPage.Vest;
            if (player.clothing.shirt == 0) modifiedPages |= EPage.Shirt;
            if (player.clothing.pants == 0) modifiedPages |= EPage.Pants;

            foreach (var groupSizes in groupSizesList)
            {
                // Get inventory pages that need to be modified
                EPage pagesToModify = modifiedPages ^ (EPage.Inventory | EPage.Storage);

                // Update all pages with the current group configuration
                modifiedPages |= ModifyPagesByRole(player, groupSizes, pagesToModify);

                // Stop if all pages are set
                if (modifiedPages == (EPage.Inventory | EPage.Storage))
                    break;
            }

            bool itemDropped = false;
            // Reset pages that were not modified
            if ((EPage.Hands & modifiedPages) != EPage.Hands)
                itemDropped |= ResetHands(player);
            if ((EPage.Backpack & modifiedPages) != EPage.Backpack)
                itemDropped |= ResetBackpack(player);
            if ((EPage.Vest & modifiedPages) != EPage.Vest)
                itemDropped |= ResetVest(player);
            if ((EPage.Shirt & modifiedPages) != EPage.Shirt)
                itemDropped |= ResetShirt(player);
            if ((EPage.Pants & modifiedPages) != EPage.Pants)
                itemDropped |= ResetPants(player);

            if (itemDropped)
                _chatMessenger.WarnInventoryItemDropped(player);
        }

        public async void ModifyPageByRoles(Player player, byte pageIndex)
        {
            // Get the player's groups ordered by priority
            List<GroupSizes> groupSizesList;
            BarricadeDrop? barricade = null;
            if (pageIndex == PlayerInventory.STORAGE)
                return;

            groupSizesList = await _sizesProvider.GetPrioritizedSizesAsync(player.channel.owner.playerID.steamID);

            foreach (var groupSizes in groupSizesList)
            {
                // Modifies the pages and returns the modified pages
                EPage modifiedPage = ModifyPageByRole(player, groupSizes, pageIndex, barricade?.asset.id ?? 0);

                // Stop if the page has been modified
                if (modifiedPage != EPage.None)
                    return;
            }
        }

        public EPage ModifyPage(Player player, byte pageIndex, byte width, byte height)
        {
            // Drop the items that are out of bounds of the page size
            bool itemDropped = DropExcessInventory(player, pageIndex, width, height);

            // Update inventory size
            _threads.RunOnMainThread(() => player.inventory.items[pageIndex].resize(width, height));

            if (itemDropped)
                _chatMessenger.WarnInventoryItemDropped(player);

            // Convert the page index to EPage
            return (EPage)(1 << pageIndex);
        }

        /// <summary>
        /// Change the size of a page under a role configuration
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="sizes"> Group configuration to use to change the page size </param>
        /// <param name="pageIndex"> Page to change the size of </param>
        /// <returns> A byte representing the indexes of the pages that have been changed </returns>
        private EPage ModifyPageByRole(Player player, GroupSizes sizes, byte pageIndex, ushort storageId = 0)
        {
            if (pageIndex == PlayerInventory.SLOTS)
            {
                // Get the configuration for this page
                PageSize pageSize = sizes.Pages.FirstOrDefault(page => page.Page == pageIndex);

                // Change the page size
                if (pageSize != null)
                    return ModifyPage(player, pageIndex, pageSize.Width, pageSize.Height);

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
            ItemStorageSize itemSize = sizes.Items.FirstOrDefault(item => item.Id == itemId);

            if (itemSize != null)
            {
                // Change the page size
                return ModifyPage(player, pageIndex, itemSize.Width, itemSize.Height);
            }
            else
            {
                // Get the default configuration for this page
                PageSize pageSize = sizes.Pages.FirstOrDefault(page => page.Page == pageIndex);

                // Change the page size
                if (pageSize != null)
                    return ModifyPage(player, pageIndex, pageSize.Width, pageSize.Height);
            }

            return EPage.None;
        }

        /// <summary>
        /// Change the size of some pages under a role configuration
        /// </summary>
        /// <param name="player"> Player of whom to change the page size </param>
        /// <param name="sizes"> Group configuration to use to change the page size </param>
        /// <param name="pagesToModify"> A byte representing the indexes of the pages that need to be changed </param>
        /// <returns> A byte representing the indexes of the pages that have been changed </returns>
        private EPage ModifyPagesByRole(Player player, GroupSizes sizes, EPage pagesToModify)
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
                modifiedPages |= ModifyPageByRole(player, sizes, pageIndex);
            }

            return modifiedPages;
        }

        /// <summary>
        /// Drop the items that are out of bounds of the inventory page size
        /// </summary>
        /// <param name="player"> Player of whom to drop the excess items of </param>
        /// <param name="pageIndex"> Page of which to drop the items from </param>
        /// <param name="width"> Width bound </param>
        /// <param name="height"> Height bound </param>
        /// <returns> True if at least one item was dropped </returns>
        private bool DropExcessInventory(Player player, byte pageIndex, byte width, byte height)
        {
            bool itemDropped = false;

            // Drop items that exceed the inventory space
            foreach (var item in player.inventory.items[pageIndex].items)
            {
                bool rotated = item.rot % 2 == 0;
                byte size_x = rotated ? item.size_x : item.size_y;
                byte size_y = !rotated ? item.size_x : item.size_y;

                if (item.x + size_x > width || item.y + size_y > height)
                {
                    player.inventory.sendDropItem(pageIndex, item.x, item.y);
                    itemDropped |= true;
                }
            }

            return itemDropped;
        }

        /// <summary>
        /// Resets a page size
        /// </summary>
        /// <param name="player"> Player of whom to reset the page </param>
        /// <param name="pageIndex"> Index of the page to reset </param>
        private bool ResetPage(Player player, EPage page)
        {
            bool itemDropped = false;
            switch (page)
            {
                case EPage.Hands:
                    itemDropped = ResetHands(player);
                    break;

                case EPage.Backpack:
                    if (player.clothing.backpack != 0)
                        itemDropped = ResetBackpack(player);
                    break;

                case EPage.Vest:
                    if (player.clothing.vest != 0)
                        itemDropped = ResetVest(player);
                    break;

                case EPage.Shirt:
                    if (player.clothing.shirt != 0)
                        itemDropped = ResetShirt(player);
                    break;

                case EPage.Pants:
                    if (player.clothing.pants != 0)
                        itemDropped = ResetPants(player);
                    break;

                default:
                    break;
            }

            return itemDropped;
        }

        /// <summary>
        /// Reset all player inventory pages to their original game size
        /// </summary>
        /// <param name="player"> Player of whom to reset the inventory </param>
        public void ResetInventorySize(Player player)
        {
            bool itemDropped = false;

            // Reset Hands
            itemDropped |= ResetHands(player);

            // Reset backpack
            if (player.clothing.backpack != 0)
                itemDropped |= ResetBackpack(player);

            // Reset vest
            if (player.clothing.vest != 0)
                itemDropped |= ResetVest(player);

            // Reset shirt
            if (player.clothing.shirt != 0)
                itemDropped |= ResetShirt(player);

            // Reset pants
            if (player.clothing.pants != 0)
                itemDropped |= ResetPants(player);

            if (itemDropped)
                _chatMessenger.WarnInventoryItemDropped(player);
        }

        /// <summary>
        /// Reset the player's hands page
        /// </summary>
        /// <param name="player"> Player of whom to reset hands page </param>
        private bool ResetHands(Player player)
        {
            bool itemDropped = DropExcessInventory(player, PlayerInventory.SLOTS, 5, 3);
            _threads.RunOnMainThread(() => player.inventory.items[PlayerInventory.SLOTS].resize(5, 3));

            return itemDropped;
        }

        /// <summary>
        /// Reset the player's backpack page
        /// </summary>
        /// <param name="player"> Player of whom to reset backpack page </param>
        private bool ResetBackpack(Player player)
        {
            byte width = player.clothing.backpackAsset.width;
            byte height = player.clothing.backpackAsset.height;

            bool itemDropped = DropExcessInventory(player, PlayerInventory.BACKPACK, width, height);
            _threads.RunOnMainThread(
                () => player.inventory.items[PlayerInventory.BACKPACK].resize(player.clothing.backpackAsset.width, player.clothing.backpackAsset.height)
            );

            return itemDropped;
        }

        /// <summary>
        /// Reset the player's vest page
        /// </summary>
        /// <param name="player"> Player of whom to reset vest page </param>
        private bool ResetVest(Player player)
        {
            byte width = player.clothing.vestAsset.width;
            byte height = player.clothing.vestAsset.height;

            bool itemDropped = DropExcessInventory(player, PlayerInventory.VEST, width, height);
            _threads.RunOnMainThread(() =>
                player.inventory.items[PlayerInventory.VEST].resize(player.clothing.vestAsset.width, player.clothing.vestAsset.height)
            );

            return itemDropped;
        }

        /// <summary>
        /// Reset the player's shirt page
        /// </summary>
        /// <param name="player"> Player of whom to reset shirt page </param>
        private bool ResetShirt(Player player)
        {
            byte width = player.clothing.shirtAsset.width;
            byte height = player.clothing.shirtAsset.height;

            bool itemDropped = DropExcessInventory(player, PlayerInventory.SHIRT, width, height);
            _threads.RunOnMainThread(() =>
                player.inventory.items[PlayerInventory.SHIRT].resize(player.clothing.shirtAsset.width, player.clothing.shirtAsset.height)
            );

            return itemDropped;
        }

        /// <summary>
        /// Reset the player's pants page
        /// </summary>
        /// <param name="player"> Player of whom to reset pants page </param>
        private bool ResetPants(Player player)
        {
            byte width = player.clothing.pantsAsset.width;
            byte height = player.clothing.pantsAsset.height;

            bool itemDropped = DropExcessInventory(player, PlayerInventory.PANTS, width, height);
            _threads.RunOnMainThread(() =>
                player.inventory.items[PlayerInventory.PANTS].resize(player.clothing.pantsAsset.width, player.clothing.pantsAsset.height)
            );

            return itemDropped;
        }
    }
}