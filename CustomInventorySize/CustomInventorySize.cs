using Rocket.Core.Plugins;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize
{
    public class CustomInventorySize : RocketPlugin
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
            Player player = PlayerTool.getPlayer(playerId);

            SendSize.Invoke(player.inventory.GetNetId(), ENetReliability.Reliable, player.inventory.channel.GetOwnerTransportConnection(), PlayerInventory.SLOTS, 1, 1);
        }
    }
}
