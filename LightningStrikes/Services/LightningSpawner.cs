using LightningStrikes.API;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LightningStrikes.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class LightningSpawner : ILightningSpawner
    {
        private const int PRE_STRIKE_DELAY = 50;

        private readonly static ClientInstanceMethod<Vector3> _sendLightningStrike = ClientInstanceMethod<Vector3>.Get(typeof(LightningWeatherComponent), "ReceiveLightningStrike");

        private readonly IWeatherProvider _weatherProvider;
        private readonly IThreadManager _threadManager;

        public LightningSpawner(IWeatherProvider weatherProvider, IThreadManager threadManager)
        {
            _threadManager = threadManager;
            _weatherProvider = weatherProvider;
        }

        public void Strike(Vector3 hitPosition, bool dealDamage = false)
        {
            _threadManager.RunOnMainThread(() => SendLightningStrike(hitPosition, dealDamage));
        }

        public void StrikeRing(Vector3[] strikePostions, int minDelay = 50, int maxDelay = 50, bool dealDamage = false)
        {
            _threadManager.RunOnMainThread(async () =>
            {
                foreach (var strikePostion in strikePostions)
                {
                    SendLightningStrike(strikePostion, dealDamage);

                    await Task.Delay(Random.Range(minDelay, maxDelay));
                }
            });
        }

        private void SendLightningStrike(Vector3 hitPosition, bool dealDamage = false)
        {
            _threadManager.RunOnMainThread(async () =>
            {
                NetId lwcNetId = _weatherProvider.SetLightningWeather();

                if (lwcNetId == NetId.INVALID)
                    return;

                await SendLightningStrike(hitPosition, lwcNetId, dealDamage);
            });
        }

        private async Task DealDamage(Vector3 hitPosition)
        {
            await Task.Delay(1000);

            DamageTool.explode(new ExplosionParameters(hitPosition, 10f, EDeathCause.BURNING)
            {
                damageOrigin = EDamageOrigin.Lightning,
                playImpactEffect = false,
                playerDamage = 75f,
                zombieDamage = 200f,
                animalDamage = 200f,
                barricadeDamage = 100f,
                structureDamage = 100f,
                vehicleDamage = 200f,
                resourceDamage = 1000f,
                objectDamage = 1000f,
                launchSpeed = 50f
            }, out List<EPlayerKill> _);
        }

        private async Task SendLightningStrike(Vector3 hitPosition, NetId lwcNetId, bool dealDamage)
        {
            _threadManager.RunOnMainThread(async () =>
            {
                await Task.Delay(PRE_STRIKE_DELAY);
                _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, Provider.GatherClientConnections(), hitPosition);
            });

            if (dealDamage)
                await DealDamage(hitPosition);
        }
    }
}