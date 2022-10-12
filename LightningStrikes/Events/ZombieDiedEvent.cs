using HarmonyLib;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningStrikes.Events
{
    [HarmonyPatch(typeof(ZombieManager), nameof(ZombieManager.sendZombieDead))]
    public class ZombieDiedEvent
    {
        public static void Prefix(Zombie zombie)
        {
            Configuration configuration = LightningStrikes.Instance.Configuration.Instance;
            LightningStrikes.Instance.LightningSpawner.SendStrike(configuration.Events.OnZombieDied, zombie.transform.position);
        }
    }
}
