using CustomInventorySize.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface IStorageModifier
    {
        void ModifyStorage(InteractableStorage storage, ushort storageId);
        void ResetStorage(BarricadeDrop barricade);
    }
}