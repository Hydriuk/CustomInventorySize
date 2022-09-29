using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;

namespace CustomInventorySize.Commands
{
    public class ResetInventorySizeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "resetinventorysize";

        public string Help => "Reset the inventory to its original game size";

        public string Syntax => "[-all | <player>]";

        public List<string> Aliases => new List<string>() { "resetinvsize" };

        public List<string> Permissions => new List<string>() { "custominventorysize.reset", "custominventorysize.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            List<Player> targets = new List<Player>();

            if (command.Length > 0)
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

            foreach (var player in targets)
                CustomInventorySize.Instance.InventoryModifier.ResetInventorySize(player);
        }
    }
}