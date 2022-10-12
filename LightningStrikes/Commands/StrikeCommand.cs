using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LightningStrikes.Commands
{
    public class StrikeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "strike";

        public string Help => "";

        public string Syntax => "[<player> | <x> <y> <z>] [-damage | -d]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

            UnturnedPlayer targetPlayer = UnturnedPlayer.FromCSteamID(CSteamID.Nil);
            bool dealDamage = false;

            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] == "-damage" || command[i] == "-d")
                {
                    dealDamage = true;
                    continue;
                }

                if (targetPlayer == null)
                {
                    targetPlayer = UnturnedPlayer.FromName(command[i]);

                    if (targetPlayer != null)
                        continue;
                }

                ChatManager.serverSendMessage($"Could not parse {command[i]}. Syntax: {Syntax}", Color.yellow, toPlayer: uPlayer.SteamPlayer());
                return;
            }

            // Set hit position
            Vector3 hitPosition;
            if (targetPlayer == null)
            {
                // Raycast target player's look
                Physics.Raycast(
                    uPlayer.Player.look.aim.position,
                    uPlayer.Player.look.aim.forward,
                    out RaycastHit hit,
                    600f,
                    RayMasks.BLOCK_COLLISION - RayMasks.SKY,
                    QueryTriggerInteraction.UseGlobal
                );
                hitPosition = hit.point;
            }
            else
            {
                // Set target player's position
                hitPosition = targetPlayer.Position;
            }

            LightningStrikes.Instance.LightningSpawner.Strike(hitPosition, dealDamage: dealDamage);
        }
    }
}
