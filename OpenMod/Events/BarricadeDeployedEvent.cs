using CustomInventorySize.API;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Building.Events;
using SDG.Unturned;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod.Events
{
    public class BarricadeDeployedEvent : IEventListener<UnturnedBarricadeDeployedEvent>
    {
        private readonly IStorageModifier _storageModifier;

        public BarricadeDeployedEvent(IStorageModifier storageModifier)
        {
            _storageModifier = storageModifier;
        }

        public Task HandleEventAsync(object? sender, UnturnedBarricadeDeployedEvent @event)
        {
            ushort itemId = ushort.Parse(@event.Buildable.Asset.BuildableAssetId);

            if (@event.Buildable.Interactable is not InteractableStorage storage)
                return Task.CompletedTask;

            _storageModifier.ModifyStorage(storage, itemId);

            return Task.CompletedTask;
        }
    }
}