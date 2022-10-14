using Cysharp.Threading.Tasks;
using LightningStrikes.API;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LightningStrikes.OpenMod.Commands
{
    [Command("strike")]
    [CommandSyntax("[<player>] [-damage | -d]")]
    [CommandDescription("Sends a lightning strike")]
    [CommandActor(typeof(UnturnedUser))]
    public class StrikeCommand : UnturnedCommand
    {
        private readonly ILightningSpawner _lightningSpawner;

        public StrikeCommand(IServiceProvider serviceProvider, ILightningSpawner lightningSpawner) : base(serviceProvider)
        {
            _lightningSpawner = lightningSpawner;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            Player? target = null;
            bool dealDamage = false;

            for (int i = 0; i < Context.Parameters.Length; i++)
            {
                if (Context.Parameters[i] == "-damage" || Context.Parameters[i] == "-d")
                {
                    dealDamage = true;
                    continue;
                }

                if (target == null)
                {
                    target = PlayerTool.getPlayer(Context.Parameters[i]);

                    if (target != null)
                        continue;
                }

                throw new CommandWrongUsageException($"Could not parse {Context.Parameters[i]}");
            }

            // Set hit position
            Vector3 hitPosition;
            if (target == null)
            {
                // Raycast target player's look
                Physics.Raycast(
                    user.Player.Player.look.aim.position,
                    user.Player.Player.look.aim.forward,
                    out RaycastHit hit,
                    1000f,
                    RayMasks.BLOCK_COLLISION - RayMasks.SKY,
                    QueryTriggerInteraction.UseGlobal
                );
                hitPosition = hit.point;
            }
            else
            {
                // Set target player's position
                hitPosition = target.transform.position;
            }

            if(hitPosition != Vector3.zero)
                _lightningSpawner.Strike(hitPosition, dealDamage: dealDamage);

            return UniTask.CompletedTask;
        }
    }
}
