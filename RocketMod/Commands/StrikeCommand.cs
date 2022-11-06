using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

namespace LightningStrikes.RocketMod.Commands
{
    public class StrikeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "strike";

        public string Help => "Sends a lightning strike.";

        public string Syntax => "[<player>] [-d]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "lightningstrikes.admin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            // current parameter index
            int i = 0;

            Player target = null;

            // Parse [<player>]
            if (command.Length > i)
            {
                target = PlayerTool.getPlayer(command[i]);
                if (target != null)
                    i++;
            }

            // Parse [-damage | -d]
            bool dealDamage = false;
            if (command.Length > i)
            {
                dealDamage = command[i] == "-damage" || command[i] == "-d";
                if (dealDamage)
                    i++;
            }

            if (command.Length > i)
            {
                ChatManager.serverSendMessage($"Wrong command syntax: {Syntax}", Color.red, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            // Set hit position
            Vector3 hitPosition;
            if (target == null)
            {
                hitPosition = LightningStrikes.Instance.StrikePositionProvider.GetLookedPosition(uPlayer.Player);
            }
            else
            {
                // Set target player's position
                hitPosition = target.transform.position;
            }

            if (hitPosition != Vector3.zero)
                LightningStrikes.Instance.LightningSpawner.Strike(hitPosition, dealDamage: dealDamage);
        }
    }
}