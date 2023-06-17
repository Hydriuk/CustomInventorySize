using CustomInventorySize.API;
using Hydriuk.UnturnedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using UnityEngine;

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

        public void ModifyInventory(Player player)
        {
            for (byte page = PlayerInventory.SLOTS; page < PlayerInventory.PAGES - 2; page++)
            {
                ModifyPage(player, page);
            }
        }

        public async void ModifyPage(Player player, byte page)
        {
            ItemBagAsset? clothAsset = GetPageItemAsset(player, page);

            Vector2 size = -Vector2.one;
            if (clothAsset != null)
            {
                size = await _sizesProvider.GetSizeAsync(player.channel.owner.playerID.steamID, clothAsset.id);
            }

            // If player has no size permission for the size
            if (size == -Vector2.one)
            {
                size = await _sizesProvider.GetSizeAsync(player.channel.owner.playerID.steamID, page);
            }

            if (size == -Vector2.one)
            {
                ResetPage(player, page);
            }
            else
            {
                ModifyPage(player, page, (byte)size.x, (byte)size.y);
            }
        }

        public void ModifyPage(Player player, byte pageIndex, byte width, byte height)
        {
            // Drop the items that are out of bounds of the page size
            DropExcessInventory(player, pageIndex, width, height);

            // Update inventory size
            _threads.RunOnMainThread(() => player.inventory.items[pageIndex].resize(width, height));
        }

        /// <summary>
        /// Drop the items that are out of bounds of the inventory page size
        /// </summary>
        /// <param name="player"> Player of whom to drop the excess items of </param>
        /// <param name="pageIndex"> Page of which to drop the items from </param>
        /// <param name="width"> Width bound </param>
        /// <param name="height"> Height bound </param>
        /// <returns> True if at least one item was dropped </returns>
        private void DropExcessInventory(Player player, byte pageIndex, byte width, byte height)
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

            if (itemDropped)
                _chatMessenger.WarnInventoryItemDropped(player);
        }

        /// <summary>
        /// Reset all player inventory pages to their original game size
        /// </summary>
        /// <param name="player"> Player of whom to reset the inventory </param>
        public void ResetInventorySize(Player player)
        {
            for (byte page = PlayerInventory.SLOTS; page < PlayerInventory.PAGES - 2; page++)
            {
                ResetPage(player, page);
            }
        }

        private void ResetPage(Player player, byte page)
        {
            byte width = 0;
            byte height = 0;

            if (page > PlayerInventory.SLOTS)
            {
                ItemBagAsset? clothAsset = GetPageItemAsset(player, page);

                if (clothAsset == null)
                    return;

                width = clothAsset.width;
                height = clothAsset.height;
            }
            else
            {
                width = 5;
                height = 3;
            }

            DropExcessInventory(player, page, width, height);

            _threads.RunOnMainThread(
                () => player.inventory.items[page].resize(width, height)
            );
        }

        private ItemBagAsset? GetPageItemAsset(Player player, byte page)
        {
            return page switch
            {
                3 => player.clothing.backpackAsset,
                4 => player.clothing.vestAsset,
                5 => player.clothing.shirtAsset,
                6 => player.clothing.pantsAsset,
                _ => null
            };
        }
    }
}