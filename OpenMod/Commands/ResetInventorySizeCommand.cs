using CustomInventorySize.API;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomInventorySize.OpenMod.Commands
{
    [Command("resetinventorysize")]
    [CommandAlias("resetinvsize")]
    [CommandSyntax("[<player> | -all]")]
    [CommandDescription("Reset the inventory to its original game size")]
    [CommandActor(typeof(UnturnedUser))]
    public class ResetInventorySizeCommand : UnturnedCommand
    {
        private readonly IInventoryModifier _inventoryModifier;

        public ResetInventorySizeCommand(IServiceProvider serviceProvider, IInventoryModifier inventoryModifier) : base(serviceProvider)
        {
            _inventoryModifier = inventoryModifier;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            List<Player> targets = new List<Player>();

            if (Context.Parameters.Count > 0)
            {
                if (Context.Parameters[0] == "-all")
                    targets.AddRange(Provider.clients.Select(sPlayer => sPlayer.player));
                else
                    targets.Add(PlayerTool.getPlayer(Context.Parameters[0]));
            }
            else
            {
                targets.Add(user.Player.Player);
            }

            foreach (var player in targets)
                _inventoryModifier.ResetInventorySize(player);

            return UniTask.CompletedTask;
        }
    }
}