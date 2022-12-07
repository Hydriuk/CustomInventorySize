using CustomInventorySize.API;
using SDG.Unturned;
using System;

namespace CustomInventorySize.RocketMod.Events
{
    public class BarricadeDeployedEvent : IDisposable
    {
        private readonly IStorageModifier _storageModifier;

        public BarricadeDeployedEvent(IStorageModifier storageModifier)
        {
            _storageModifier = storageModifier;

            BarricadeManager.onBarricadeSpawned += OnBarricadeDeploying;
        }

        public void Dispose()
        {
            BarricadeManager.onBarricadeSpawned -= OnBarricadeDeploying;
        }

        private void OnBarricadeDeploying(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.interactable is not InteractableStorage storage)
                return;

            _storageModifier.ModifyStorage(storage, drop.asset.id);
        }
    }
}