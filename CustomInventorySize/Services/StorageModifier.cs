using CustomInventorySize.API;
using Hydriuk.UnturnedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomInventorySize.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class StorageModifier : IStorageModifier
    {
        private readonly ISizesProvider _sizesProvider;
        private readonly IThreadAdapter _threads;
        private readonly IInventoryModifier _inventoryModifier;

        public StorageModifier(ISizesProvider sizesProvider, IThreadAdapter threads, IInventoryModifier inventoryModifier)
        {
            _sizesProvider = sizesProvider;
            _threads = threads;
            _inventoryModifier = inventoryModifier;
        }

        public async void ModifyStorage(InteractableStorage storage, ushort storageId)
        {
            Vector2 size = await _sizesProvider.GetSizeAsync(storage.owner, storageId);

            if (size == -Vector2.one)
                return;

            ModifyStorage(storage, (byte)size.x, (byte)size.y);
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