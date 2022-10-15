using Cysharp.Threading.Tasks;
using LightningStrikes.API;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;

namespace LightningStrikes.OpenMod.Commands
{
    [Command("strikering")]
    [CommandAlias("striker")]
    [CommandSyntax("[<player>] <amount> <radius> [<minDelay> <maxDelay>] [-d] [-r | -c]")]
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

            if (Context.Parameters.Length < 2)
                throw new CommandWrongUsageException(Context);

            // Current parameter index
            int i = 0;

            // Parse [<player>]
            Player? target = PlayerTool.getPlayer(Context.Parameters[i]);
            if (target != null)
                i++;

            // Parse <amount>
            if (!int.TryParse(Context.Parameters[i], out int amount))
                throw new CommandWrongUsageException("<amount> must be a number");
            if (amount <= 0)
                throw new CommandWrongUsageException("<amount> must be a greater or equal to 1");
            i++;

            // Parse <radius>
            if (!float.TryParse(Context.Parameters[i], out float radius))
                throw new CommandWrongUsageException("<radius> must be a number");
            i++;

            // Parse [<minDelay> <maxDelay>]
            if (Context.Parameters.Length > i + 1 && int.TryParse(Context.Parameters[i], out int minDelay) && int.TryParse(Context.Parameters[i + 1], out int maxDelay))
            {
                i += 2;
            }
            else
            {
                minDelay = -1;
                maxDelay = -1;
            }

            // Parse flags
            bool dealDamage = false;
            bool random = false;
            bool circle = false;
            for (int j = i; j < Context.Parameters.Length; j++)
            {
                // Parse [-damage | -d]
                if (Context.Parameters[j] == "-damage" || Context.Parameters[j] == "-d")
                {
                    dealDamage = true;
                    i++;
                }

                // Parse [-random | -r]
                if (Context.Parameters[j] == "-random" || Context.Parameters[j] == "-r")
                {
                    random = true;
                    i++;
                }

                // Parse [-circle | -c]
                if (Context.Parameters[j] == "-circle" || Context.Parameters[j] == "-c")
                {
                    circle = true;
                    i++;
                }
            }

            if (Context.Parameters.Length > i)
                throw new CommandWrongUsageException(Context);

            // Update target
            if (target == null)
                target = user.Player.Player;

            // Strike
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