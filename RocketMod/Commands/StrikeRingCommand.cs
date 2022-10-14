using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LightningStrikes.RocketMod.Commands
{
    public class StrikeRingCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "strikering";

        public string Help => "Sends multiple lightning strikes around a ring.";

        public string Syntax => "<radius> [<player>] <amount> [<minDelay> <maxDelay>] [-damage | -d] [-random | -r] [-circle | -c]";

        public List<string> Aliases => new List<string>() { "striker" };

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            UnturnedPlayer targetPlayer = UnturnedPlayer.FromCSteamID(CSteamID.Nil);
            bool dealDamage = false;
            bool random = false;
            bool circle = false;
            float radius = -1f;
            int amount = -1;
            int minDelay = -1;
            int maxDelay = -1;

            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] == "-damage" || command[i] == "-d")
                {
                    dealDamage = true;
                    continue;
                }

                if (command[i] == "-random" || command[i] == "-r")
                {
                    random = true;
                    continue;
                }

                if (command[i] == "-circle" || command[i] == "-c")
                {
                    circle = true;
                    continue;
                }

                if (radius == -1)
                {
                    bool success = command.Length > i && float.TryParse(command[i], out radius);
 
                    if (success)
                        continue;

                    ChatManager.serverSendMessage($"Radius must be a number. Was '{command[i]}'. Syntax: {Syntax}", Color.yellow, toPlayer: uPlayer.SteamPlayer());
                    return;
                }

                if (targetPlayer == null)
                {
                    targetPlayer = UnturnedPlayer.FromName(command[i]);

                    if (targetPlayer != null)
                        continue;
                }

                if (amount == -1)
                {
                    bool success = int.TryParse(command[i], out amount);

                    if (success)
                    {
                        if(command.Length > i + 2)
                        {
                            success = int.TryParse(command[i+1], out minDelay) && int.TryParse(command[i+2], out maxDelay);

                            if (success)
                                i += 2;
                        }

                        continue;
                    }
                }

                ChatManager.serverSendMessage($"Could not parse {command[i]}. Syntax: {Syntax}", Color.yellow, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            LightningStrikes.Instance.LightningSpawner.StrikeRing(
                uPlayer.Position, 
                amount, 
                radius, 
                minDelay != -1 ? minDelay : 50, 
                maxDelay != -1 ? maxDelay : 50, 
                dealDamage,
                random,
                circle
            );
        }
    }
}
