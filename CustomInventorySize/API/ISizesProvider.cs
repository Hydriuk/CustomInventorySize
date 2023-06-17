using System.Threading.Tasks;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using Steamworks;
using UnityEngine;

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface ISizesProvider
    {
        Task<Vector2> GetSizeAsync(CSteamID playerId, byte page);
        Task<Vector2> GetSizeAsync(CSteamID playerId, ushort itemID);
    }
}