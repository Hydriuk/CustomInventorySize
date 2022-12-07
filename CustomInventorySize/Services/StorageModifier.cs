using CustomInventorySize.API;
using CustomInventorySize.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;

namespace CustomInventorySize.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class StorageModifier : IStorageModifier
    {
        private readonly ISizesProvider _sizesProvider;
        private readonly IThreadAdapter _threads;

        public StorageModifier(ISizesProvider sizesProvider, IThreadAdapter threads)
        {
            _sizesProvider = sizesProvider;
            _threads = threads;
        }

        public async void ModifyStorage(InteractableStorage storage, ushort storageId)
        {
            List<GroupSizes> groupSizes = await _sizesProvider.GetPrioritizedSizesAsync(storage.owner);

            GroupSizes sizes = groupSizes.FirstOrDefault(sizes => sizes.Items.Any(item => item.Id == storageId) || sizes.Pages.Any(page => page.Page == PlayerInventory.STORAGE));

            if (sizes == null)
                return;

            ItemStorageSize size = sizes.Items.FirstOrDefault(item => item.Id == storageId);

            if (size != null)
            {
                ModifyStorage(storage, size.Width, size.Height);
                return;
            }

            PageSize pageSize = sizes.Pages.FirstOrDefault(page => page.Page == PlayerInventory.STORAGE);

            if (pageSize != null)
            {
                ModifyStorage(storage, pageSize.Width, pageSize.Height);
                return;
            }
        }

        private void ModifyStorage(InteractableStorage storage, byte width, byte height)
        {
            // Drop the items that are out of bounds of the page size
            bool itemDropped = DropExcessStorage(storage, width, height);

            // Update inventory size
            _threads.RunOnMainThread(() => storage.items.resize(width, height));
        }

        /// <summary>
        /// Reset the player's storage page
        /// </summary>
        /// <param name="player"> Player of whom to reset storage page </param>
        public void ResetStorage(BarricadeDrop barricade)
        {
            ItemStorageAsset asset = (ItemStorageAsset)barricade.asset;

            if (barricade.interactable is not InteractableStorage storage)
                return;

            bool itemDropped = DropExcessStorage(storage, asset.storage_x, asset.storage_y);

            _threads.RunOnMainThread(() => storage.items.resize(asset.storage_x, asset.storage_y));
        }

        /// <summary>
        /// Drop the items that are out of bounds of the storage page size
        /// </summary>
        /// <param name="player"> Player of whom to drop the excess items of </param>
        /// <param name="width"> Width bound </param>
        /// <param name="height"> Height bound </param>
        /// <returns> True if at least one item was dropped </returns>
        private bool DropExcessStorage(InteractableStorage storage, byte width, byte height)
        {
            bool itemDropped = false;
            List<ItemJar> items = new List<ItemJar>();

            // Drop items that exceed the inventory space
            foreach (var item in storage.items.items)
            {
                bool rotated = item.rot % 2 == 0;
                byte size_x = rotated ? item.size_x : item.size_y;
                byte size_y = !rotated ? item.size_x : item.size_y;

                // ItemJar size takes rotation into account
                if (!(item.x + size_x > width || item.y + size_y > height))
                    continue;

                // Drop item on floor
                ItemManager.dropItem(item.item, storage.transform.position + storage.transform.forward * 0.5f, true, true, false);

                items.Add(item);

                itemDropped |= true;
            }

            // Remove dropped items from the storage
            _threads.RunOnMainThread(() =>
            {
                foreach (var item in items)
                {
                    byte index = storage.items.getIndex(item.x, item.y);

                    storage.items.removeItem(index);
                }
            });

            return itemDropped;
        }
    }
}