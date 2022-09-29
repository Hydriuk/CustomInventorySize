using System.Collections.Generic;
using CustomInventorySize.Models;
#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using Steamworks;

namespace CustomInventorySize.API
{
#if OPENMOD
    [Service]
#endif
    public interface ISizesProvider
    {
        /// <summary>
        /// Get configured sizes for the player's groups
        /// </summary>
        /// <param name="playerId"> Id of the player of whom to get groups </param>
        /// <returns> Sizes ordered by group priority </returns>
        List<GroupSizes> GetPrioritizedSizes(CSteamID playerId);

        /// <summary>
        /// Get configured sizes for the player's groups
        /// </summary>
        /// <param name="player"> Player of whom to get groups </param>
        /// <returns> Sizes ordered by group priority </returns>
        List<GroupSizes> GetPrioritizedSizes(Player playerId);
    }
}