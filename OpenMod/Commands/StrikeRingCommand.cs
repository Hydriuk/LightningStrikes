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

namespace LightningStrikes.OpenMod.Commands
{
    [Command("strikering")]
    [CommandAlias("striker")]
    [CommandSyntax("<radius> [<player>] <amount> [<minDelay> <maxDelay>] [-damage | -d] [-random | -r] [-circle | -c]")]
    [CommandDescription("Sends multiple lightning strikes around a ring.")]
    [CommandActor(typeof(UnturnedUser))]
    public class StrikeRingCommand : UnturnedCommand
    {
        private readonly ILightningSpawner _lightningSpawner;

        public StrikeRingCommand(IServiceProvider serviceProvider, ILightningSpawner lightningSpawner) : base(serviceProvider)
        {
            _lightningSpawner = lightningSpawner;
        }

        protected override UniTask OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;

            Player? target = null;
            bool dealDamage = false;
            bool random = false;
            bool circle = false;
            float radius = -1f;
            int amount = -1;
            int minDelay = -1;
            int maxDelay = -1;

            for (int i = 0; i < Context.Parameters.Length; i++)
            {
                if (Context.Parameters[i] == "-damage" || Context.Parameters[i] == "-d")
                {
                    dealDamage = true;
                    continue;
                }

                if (Context.Parameters[i] == "-random" || Context.Parameters[i] == "-r")
                {
                    random = true;
                    continue;
                }

                if (Context.Parameters[i] == "-circle" || Context.Parameters[i] == "-c")
                {
                    circle = true;
                    continue;
                }

                if (radius == -1)
                {
                    bool success = Context.Parameters.Length > i && float.TryParse(Context.Parameters[i], out radius);

                    if (success)
                        continue;

                    throw new CommandWrongUsageException("Radius must be a number");
                }

                if (target == null)
                {
                    target = PlayerTool.getPlayer(Context.Parameters[i]);

                    if (target != null)
                        continue;
                }

                if (amount == -1)
                {
                    bool success = int.TryParse(Context.Parameters[i], out amount);

                    if (success)
                    {
                        if (Context.Parameters.Length > i + 2)
                        {
                            success = int.TryParse(Context.Parameters[i + 1], out minDelay) && int.TryParse(Context.Parameters[i + 2], out maxDelay);

                            if (success)
                                i += 2;
                        }

                        continue;
                    }
                }

                throw new CommandWrongUsageException($"Could not parse {Context.Parameters[i]}");
            }

            if (amount == -1)
                throw new CommandWrongUsageException("Amount must be greater or equal to 1");

            if (target == null)
                target = user.Player.Player;

            _lightningSpawner.StrikeRing(
                target.transform.position,
                amount,
                radius,
                minDelay != -1 ? minDelay : 50,
                maxDelay != -1 ? maxDelay : 50,
                dealDamage,
                random,
                circle
            );

            return UniTask.CompletedTask;
        }
    }
}
