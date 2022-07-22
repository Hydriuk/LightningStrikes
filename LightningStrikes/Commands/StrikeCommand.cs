using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningStrikes.Commands
{
    public class StrikeCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "strike";

        public string Help => "";

        public string Syntax => "<player> [<shiftX> <shiftY> <shiftZ>] [<playerToSee>] [{-damage | -d}] [{-playerLookPosition | -plp}]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            LightningStrikes.LightningSpawner.SendLightningStrike(player.Position);
        }
    }
}
