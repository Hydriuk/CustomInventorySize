using CustomInventorySize.API;
using CustomInventorySize.Models;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Linq;

namespace CustomInventorySize.RocketMod.Services
{
    public class SizesProvider : ISizesProvider
    {
        private readonly Dictionary<string, GroupSizes> _groupSizesProvider;

        public SizesProvider(RocketConfiguration configuration)
        {
            _groupSizesProvider = configuration.Groups.ToDictionary(group => group.GroupName);
        }

        /// <summary>
        /// Get configured sizes for the player's groups
        /// </summary>
        /// <param name="playerId"> Id of the player of whom to get groups </param>
        /// <returns> Sizes ordered by group priority </returns>
        public List<GroupSizes> GetPrioritizedSizes(CSteamID playerId)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(playerId);

            return GetPrioritizedSizes(uPlayer);
        }

        /// <summary>
        /// Get configured sizes for the player's groups
        /// </summary>
        /// <param name="player"> Player of whom to get groups </param>
        /// <returns> Sizes ordered by group priority </returns>
        public List<GroupSizes> GetPrioritizedSizes(Player player)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);

            return GetPrioritizedSizes(uPlayer);
        }

        /// <summary>
        /// Get configured sizes for the player's groups
        /// </summary>
        /// <param name="uPlayer"> Rocket player of whom to get groups </param>
        /// <returns> Sizes ordered by group priority </returns>
        private List<GroupSizes> GetPrioritizedSizes(UnturnedPlayer uPlayer)
        {
            var rocketGroups = R.Permissions
                .GetGroups(uPlayer, true)
                .OrderBy(p => p.Priority);

            List<GroupSizes> sizesOrderedList = new List<GroupSizes>();
            foreach (var rocketGroup in rocketGroups)
            {
                if (_groupSizesProvider.TryGetValue(rocketGroup.Id, out GroupSizes sizes))
                    sizesOrderedList.Add(sizes);
            }

            return sizesOrderedList;
        }
    }
}