using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomInventorySize.RocketMod.Commands
{
    public class SetInventorySizeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "setinventorysize";

        public string Help => "Set the size of an inventory slot";

        public string Syntax => "[<player> | -all] <slotIndex> <width> <height>";

        public List<string> Aliases => new List<string>() { "setinvsize" };

        public List<string> Permissions => new List<string>() { "custominventorysize.set", "custominventorysize.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            if (command.Length < 3)
            {
                ChatManager.serverSendMessage($"Wrong syntax : {Syntax}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            int i = 0;
            List<Player> targets = new List<Player>();
            if (command.Length > 3)
            {
                if (command[0] == "-all")
                    targets.AddRange(Provider.clients.Select(sPlayer => sPlayer.player));
                else
                    targets.Add(PlayerTool.getPlayer(command[0]));
            }
            else
            {
                targets.Add(uPlayer.Player);
            }

            if (!byte.TryParse(command[i], out byte pageIndex) ||
                !byte.TryParse(command[i + 1], out byte width) ||
                !byte.TryParse(command[i + 2], out byte height))
            {
                ChatManager.serverSendMessage($"SlotIndex, width and height must be numbers : {Syntax}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            if (pageIndex < 2 || pageIndex > 6)
            {
                ChatManager.serverSendMessage($"Slot index must be a number bewteen 2 and 6 (included)", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            foreach (var player in targets)
                CustomInventorySize.Instance.InventoryModifier.SendModifyPage(player, pageIndex, width, height);
        }
    }
}