using Cysharp.Threading.Tasks;
using LightningStrikes.API;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly static ClientInstanceMethod<Vector3> _sendLightningStrike = ClientInstanceMethod<Vector3>.Get(typeof(LightningWeatherComponent), "ReceiveLightningStrike");

        private readonly IWeatherProvider _weatherProvider;

        public LightningSpawner(IWeatherProvider weatherProvider)
        {
            _weatherProvider = weatherProvider;
        }

        public void Strike(Vector3 hitPosition, bool dealDamage = false)
        {
            Task.Run(async () =>
            {
                await UniTask.SwitchToMainThread();

                await SendLightningStrike(hitPosition, dealDamage);
            });
        }

        public void StrikeRing(Vector3[] strikePostions, int minDelay = 50, int maxDelay = 50, bool dealDamage = false)
        {
            Task.Run(async () =>
            {
                await UniTask.SwitchToMainThread();

                foreach (var strikePostion in strikePostions)
                {
                    await SendLightningStrike(strikePostion, dealDamage);

                    await Task.Delay(Random.Range(minDelay, maxDelay));
                }
            });
        }

        private async Task SendLightningStrike(Vector3 hitPosition, bool dealDamage = false)
        {
            NetId lwcNetId = await _weatherProvider.SetLightningWeather();

            if (lwcNetId == NetId.INVALID)
                return;

            SendLightningStrike(hitPosition, lwcNetId, dealDamage);
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

        private void SendLightningStrike(Vector3 hitPosition, NetId lwcNetId, bool dealDamage)
        {
            _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, Provider.GatherClientConnections(), hitPosition);

            if (dealDamage)
                Task.Run(async () => await DealDamage(hitPosition));
        }
    }
}