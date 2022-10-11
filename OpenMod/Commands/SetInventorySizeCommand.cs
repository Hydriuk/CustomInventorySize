using CustomInventorySize.API;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomInventorySize.OpenMod.Commands
{
    [Command("setinventorysize")]
    [CommandAlias("setinvsize")]
    [CommandSyntax("[<player> | -all] <slotIndex> <width> <height>")]
    [CommandDescription("Set the size of an inventory slot")]
    [CommandActor(typeof(UnturnedUser))]
    public class SetInventorySizeCommand : UnturnedCommand
    {
        private readonly IInventoryModifier _inventoryModifier;

        public SetInventorySizeCommand(IServiceProvider serviceProvider, IInventoryModifier inventoryModifier) : base(serviceProvider)
        {
            _inventoryModifier = inventoryModifier;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            if (Context.Parameters.Count < 3)
                throw new CommandWrongUsageException(Context);

            int i = 0;
            List<Player> targets = new List<Player>();
            if (Context.Parameters.Count > 3)
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

            if (!byte.TryParse(Context.Parameters[i], out byte pageIndex) ||
                !byte.TryParse(Context.Parameters[i + 1], out byte width) ||
                !byte.TryParse(Context.Parameters[i + 2], out byte height))
                throw new UserFriendlyException("SlotIndex, width and height must be numbers");

            if (pageIndex < 2 || pageIndex > 6)
                throw new UserFriendlyException("SlotIndex must be a number bewteen 2 and 6 (included)");

            foreach (var player in targets)
                _inventoryModifier.ModifyPage(player, pageIndex, width, height);

            return UniTask.CompletedTask;
        }
    }
}