using Cysharp.Threading.Tasks;
using LightningStrikes.API;
using LightningStrikes.Services;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using UnityEngine;

namespace LightningStrikes.OpenMod.Commands
{
    [Command("strike")]
    [CommandSyntax("[<player>] [-d]")]
    [CommandDescription("Sends a lightning strike.")]
    [CommandActor(typeof(UnturnedUser))]
    public class StrikeCommand : UnturnedCommand
    {
        private readonly ILightningSpawner _lightningSpawner;
        private readonly IStrikePositionProvider _strikePositionProvider;

        public StrikeCommand(IServiceProvider serviceProvider, ILightningSpawner lightningSpawner, IStrikePositionProvider strikePositionProvider) : base(serviceProvider)
        {
            _lightningSpawner = lightningSpawner;
            _strikePositionProvider = strikePositionProvider;
        }

        protected override async UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            // current parameter index
            int i = 0;

            // Parse [<player>]
            Player? target = null;
            if (Context.Parameters.Length > i)
            {
                target = PlayerTool.getPlayer(Context.Parameters[i]);
                if (target != null)
                    i++;
            }

            // Parse [-damage | -d]
            bool dealDamage = false;
            if (Context.Parameters.Length > i)
            {
                dealDamage = Context.Parameters[i] == "-damage" || Context.Parameters[i] == "-d";
                if (dealDamage)
                    i++;
            }

            if (Context.Parameters.Length > i)
                throw new CommandWrongUsageException(Context);

            // Set hit position
            Vector3 hitPosition;
            if (target == null)
            {
                hitPosition = _strikePositionProvider.GetLookedPosition(user.Player.Player);
            }
            else
            {
                // Set target player's position
                hitPosition = target.transform.position;
            }

            if (hitPosition != Vector3.zero)
            {
                _lightningSpawner.Strike(hitPosition, dealDamage: dealDamage);
            }
        }
    }
}