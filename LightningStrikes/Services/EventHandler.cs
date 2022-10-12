using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LightningStrikes.Configuration;

namespace LightningStrikes.Services
{
    public class EventHandler : MonoBehaviour, IDisposable
    {
        private Configuration Configuration;

        public EventHandler()
        {
        }

        public void Load(Configuration configuration)
        {
            Configuration = configuration;

            //DamageTool.damageZombieRequested += OnZombieDied;
            DamageTool.damageAnimalRequested += OnAnimalDied;

            Provider.onBanPlayerRequestedV2 += OnPlayerBan;

            Provider.onServerDisconnected += OnPlayerDisconnected;

            Provider.onServerConnected += OnPlayerConnected;
            //Provider.onLoginSpawning;
            //PlayerLife.OnRevived_Global;

            //PlayerLife.onPlayerDied;
            PlayerLife.OnPreDeath += OnPlayerDying;

            
        }

        #region Event Handlers
        public void OnZombieDied(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            Console.WriteLine("OnZombieDied");
            Console.WriteLine(parameters.zombie.isDead);
            if (parameters.zombie.isDead)
                LightningStrikes.Instance.LightningSpawner.SendStrike(Configuration.Events.OnZombieDied, parameters.zombie.transform.position);
        }

        public void OnMegaZombieDied(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            Console.WriteLine("OnMegaZombieDied");
            Console.WriteLine((parameters.zombie.isMega || parameters.zombie.isBoss) && parameters.zombie.isDead);
            if ((parameters.zombie.isMega || parameters.zombie.isBoss) && parameters.zombie.isDead)
                LightningStrikes.Instance.LightningSpawner.SendStrike(Configuration.Events.OnMegaZombieDied, parameters.zombie.transform.position);
        }

        public void OnAnimalDied(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            Console.WriteLine("OnAnimalDied");
            Console.WriteLine(parameters.animal.isDead);
            if(parameters.animal.isDead)
                LightningStrikes.Instance.LightningSpawner.SendStrike(Configuration.Events.OnAnimalDied, parameters.animal.transform.position);
        }

        public void OnPlayerBan(CSteamID instigator, CSteamID playerToBan, uint ipToBan, IEnumerable<byte[]> hwidsToBan, ref string reason, ref uint duration, ref bool shouldVanillaBan)
        {
            Console.WriteLine("OnPlayerBan");
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(playerToBan);

            LightningStrikes.Instance.LightningSpawner.SendStrike(Configuration.Events.OnPlayerBanned, player.Position);
        }

        public void OnPlayerConnected(CSteamID steamID)
        {
            Console.WriteLine("OnPlayerConnected");
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(steamID);

            LightningStrikes.Instance.LightningSpawner.SendStrike(Configuration.Events.OnPlayerBanned, player.Position);
        }

        public void OnPlayerDisconnected(CSteamID steamID)
        {
            Console.WriteLine("OnPlayerDisconnected");
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(steamID);

            LightningStrikes.Instance.LightningSpawner.SendStrike(Configuration.Events.OnPlayerBanned, player.Position);
        }

        public void OnPlayerDying(PlayerLife playerLife)
        {
            Console.WriteLine("OnPlayerDying");
            LightningStrikes.Instance.LightningSpawner.SendStrike(Configuration.Events.OnPlayerBanned, playerLife.player.transform.position);
        }
        #endregion


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
