using CustomInventorySize.API;
using CustomInventorySize.Models;
using Cysharp.Threading.Tasks.Triggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.OpenMod.Services
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class SizesProvider : ISizesProvider
    {
        private readonly Dictionary<string, GroupSizes> _groupSizesProvider;
        private readonly IPermissionRoleStore _permissionRoleStore;
        private readonly UnturnedUserProvider _unturnedUserProvider;

        public SizesProvider(IConfiguration configurator, IPermissionRoleStore permissionRoleStore, IUserManager userManager)
        {
            Configuration configuration = new Configuration();
            configurator.Bind(configuration);

            _groupSizesProvider = configuration.Groups.ToDictionary(group => group.GroupName);
            _permissionRoleStore = permissionRoleStore;
            _unturnedUserProvider = (UnturnedUserProvider)userManager.UserProviders.First(provider => provider is UnturnedUserProvider);
        }

        public async Task<List<GroupSizes>> GetPrioritizedSizesAsync(CSteamID playerId)
        {
            UnturnedUser? user = _unturnedUserProvider.GetUser(playerId);

            if (user == null)
                throw new NullReferenceException();

            return await GetPrioritizedSizesAsync(user);
        }

        public async Task<List<GroupSizes>> GetPrioritizedSizesAsync(Player player)
        {
            UnturnedUser? user = _unturnedUserProvider.GetUser(player);

            if (user == null)
                throw new NullReferenceException();

            return await GetPrioritizedSizesAsync(user);
        }

        private async Task<List<GroupSizes>> GetPrioritizedSizesAsync(UnturnedUser uPlayer)
        {
            var openmodRoles = (await _permissionRoleStore.GetRolesAsync(uPlayer, true))
                .OrderBy(role => role.Priority);

            List<GroupSizes> sizesOrderedList = new List<GroupSizes>();
            foreach (var openmodRole in openmodRoles)
            {
                if (_groupSizesProvider.TryGetValue(openmodRole.Id, out GroupSizes sizes))
                    sizesOrderedList.Add(sizes);
            }

            return sizesOrderedList;
        }
    }
}
