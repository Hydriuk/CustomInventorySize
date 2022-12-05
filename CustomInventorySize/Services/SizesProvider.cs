using CustomInventorySize.API;
using CustomInventorySize.Models;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using PermissionsModule.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class SizesProvider : ISizesProvider
    {
        private readonly Dictionary<string, GroupSizes> _groupSizesProvider;
        private readonly IPermissionsAdapter _permissionsAdapter;

        public SizesProvider(IConfigurationAdapter configuration, IPermissionsAdapter permissionsAdapter)
        {
            _groupSizesProvider = configuration.Groups.ToDictionary(group => group.PermissionName);
            _permissionsAdapter = permissionsAdapter;
        }

        public async Task<List<GroupSizes>> GetPrioritizedSizesAsync(CSteamID playerId)
        {
            IEnumerable<string> roles = await _permissionsAdapter.PrioritizePermissions(playerId, _groupSizesProvider.Keys);

            List<GroupSizes> sizesOrderedList = new List<GroupSizes>();
            foreach (var role in roles)
            {
                if (_groupSizesProvider.TryGetValue(role, out GroupSizes sizes))
                    sizesOrderedList.Add(sizes);
            }

            return sizesOrderedList;
        }
    }
}
