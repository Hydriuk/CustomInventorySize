using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomInventorySize
{
    public class CustomInventorySize : RocketPlugin<Configuration>
    {
        private static readonly ClientInstanceMethod<byte, byte, byte> SendSize = ClientInstanceMethod<byte, byte, byte>.Get(typeof(PlayerInventory), "ReceiveSize");

        protected override void Load()
        {
            Provider.onServerConnected += OnPlayerConnected;
        }

        protected override void Unload()
        {
            Provider.onServerConnected -= OnPlayerConnected;
        }

        private void OnPlayerConnected(CSteamID playerId)
        {
            var sizesProvider = Configuration.Instance.Groups.ToDictionary(group => group.GroupName);
            var uPlayer = UnturnedPlayer.FromCSteamID(playerId);

            var groups = R.Permissions.GetGroups(uPlayer, true);

            groups = groups.OrderBy(p => p.Priority).ToList();

            foreach (var group in groups)
            {
                if (!sizesProvider.ContainsKey(group.Id))
                    continue;

                foreach (var pageSize in sizesProvider[group.Id].Pages)
                {
                    SendSize.Invoke(
                        uPlayer.Player.inventory.GetNetId(),
                        ENetReliability.Reliable,
                        uPlayer.Player.inventory.channel.GetOwnerTransportConnection(),
                        pageSize.PageIndex,
                        pageSize.Width,
                        pageSize.Height);
                }

                break;
            }
        }
    }
}
