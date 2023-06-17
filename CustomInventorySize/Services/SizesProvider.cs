using CustomInventorySize.API;
using Hydriuk.UnturnedModules.Adapters;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomInventorySize.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class SizesProvider : ISizesProvider
    {
        private readonly IPermissionAdapter _permissionsAdapter;

        private const string _prefix = "inventorysize";

        public SizesProvider(IPermissionAdapter permissionsAdapter)
        {
            _permissionsAdapter = permissionsAdapter;
        }

        public Task<Vector2> GetSizeAsync(CSteamID playerId, byte page)
        {
            string prefix = $"{_prefix}.page.{page}.";

            return GetAndParsePermissions(playerId, prefix);
        }

        public Task<Vector2> GetSizeAsync(CSteamID playerId, ushort itemID)
        {
            string prefix = $"{_prefix}.item.{itemID}.";

            return GetAndParsePermissions(playerId, prefix);
        }

        private async Task<Vector2> GetAndParsePermissions(CSteamID playerId, string prefix)
        {
            IEnumerable<string> permissions = await _permissionsAdapter.GetPrioritizedPermissions(playerId, @$"^{prefix}\d*.\d*$");

            string permission = permissions.FirstOrDefault();

            if (permission == null)
                return -Vector2.one;

            // Parse size
            string sizeString = permission.Replace(prefix, string.Empty);

            string[] sizes = sizeString.Split('.');

            if (!byte.TryParse(sizes[0], out byte x) || !byte.TryParse(sizes[1], out byte y))
                return -Vector2.one;

            return new Vector2(x, y);
        }
    }
}
