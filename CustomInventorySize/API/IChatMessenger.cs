#if OPENMOD
using OpenMod.API.Ioc;
#endif

using SDG.Unturned;

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface IChatMessenger
    {
        void WarnInventoryItemDropped(Player player);
        void WarnStorageItemDropped(Player player);
    }
}