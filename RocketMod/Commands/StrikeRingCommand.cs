using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace LightningStrikes.RocketMod.Commands
{
    public class StrikeRingCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "strikering";

        public string Help => "Sends multiple lightning strikes around a ring.";

        public string Syntax => "[<player>] <amount> <radius> [<minDelay> <maxDelay>] [-d] [-r | -c] [-g]";

        public List<string> Aliases => new List<string>() { "striker" };

        public List<string> Permissions => new List<string>() { "lightningstrikes.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            if (command.Length < 2)
            {
                ChatManager.serverSendMessage("Not enough parameters", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            // Current parameter index
            int i = 0;

            // Parse [<player>]
            Player target = PlayerTool.getPlayer(command[i]);
            if (target != null)
                i++;

            // Parse <amount>
            if (!int.TryParse(command[i], out int amount))
            {
                ChatManager.serverSendMessage("<amount> must be a number", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }
            if (amount <= 0)
            {
                ChatManager.serverSendMessage("<amount> must be a greater or equal to 1", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }
            i++;

            // Parse <radius>
            if (!float.TryParse(command[i], out float radius))
            {
                ChatManager.serverSendMessage("<radius> must be a number", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }
            i++;

            // Parse [<minDelay> <maxDelay>]
            if (command.Length > i + 1 && int.TryParse(command[i], out int minDelay) && int.TryParse(command[i + 1], out int maxDelay))
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
            bool hitGround = false;
            for (int j = i; j < command.Length; j++)
            {
                // Parse [-damage | -d]
                if(command[j] == "-damage" || command[j] == "-d")
                {
                    dealDamage = true;
                    i++;
                }

                // Parse [-random | -r]
                if (command[j] == "-random" || command[j] == "-r")
                {
                    random = true;
                    i++;
                }

                // Parse [-circle | -c]
                if(command[j] == "-circle" || command[j] == "-c")
                {
                    circle = true;
                    i++;
                }

                // Parse [-ground | -g]
                if (command[j] == "-ground" || command[j] == "-g")
                {
                    hitGround = true;
                    i++;
                }
            }

            if (command.Length > i)
            {
                ChatManager.serverSendMessage($"Wrong command syntax: {Syntax}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            // Update target
            if (target == null)
                target = uPlayer.Player;

            Vector3[] strikePositions = LightningStrikes.Instance.StrikePositionProvider.GetPositions(target.transform.position, amount, radius, random, circle, hitGround);

            // Strike
            LightningStrikes.Instance.LightningSpawner.StrikeRing(
                strikePositions,
                minDelay != -1 ? minDelay : 50,
                maxDelay != -1 ? maxDelay : 50,
                dealDamage
            );
        }
    }
}