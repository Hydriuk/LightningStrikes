using LightningStrikes.API;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.NetTransport;
using SDG.Unturned;
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
        private const int POST_STRIKE_DELAY = 2000;

        private readonly static ClientInstanceMethod<Vector3> _sendLightningStrike = ClientInstanceMethod<Vector3>.Get(typeof(LightningWeatherComponent), "ReceiveLightningStrike");

        private readonly IWeatherProvider _weatherProvider;
        private readonly IThreadManager _threadManager;


        private readonly float _lightningRangeRadius = -1f;
        private static int _currentLighningCount = 0;
        private static NetId LWCNetId = NetId.INVALID;

        public LightningSpawner(IWeatherProvider weatherProvider, IThreadManager threadManager)
        {
            _weatherProvider = weatherProvider;
            _threadManager = threadManager;
        }

        public bool Strike(Vector3 hitPosition, bool dealDamage = false)
        {
            return SendLightningStrike(hitPosition, dealDamage);
            //if (player == null)
            //else
            //    return SendLightningStrike(hitPosition, player, dealDamage);
        }

        public void StrikeRing(Vector3[] strikePostions, int minDelay = 50, int maxDelay = 50, bool dealDamage = false)
        {
            Task.Run(async () =>
            {
                foreach (var strikePostion in strikePostions)
                {
                    SendLightningStrike(strikePostion, dealDamage);
                    await Task.Delay(Random.Range(minDelay, maxDelay));
                }
            });
        }

        private bool SendLightningStrike(Vector3 hitPosition, bool dealDamage = false)
        {
            if (LWCNetId == NetId.INVALID)
                LWCNetId = _weatherProvider.SetLightningWeather();

            if (LWCNetId == NetId.INVALID)
                return false;

            if (_lightningRangeRadius > 0f)
                _ = SendLightningStrike(hitPosition, Provider.EnumerateClients_WithinSphere(hitPosition, _lightningRangeRadius), LWCNetId);
            else
                _ = SendLightningStrike(hitPosition, Provider.EnumerateClients(), LWCNetId);

            if (dealDamage)
                _threadManager.Execute(DealDamage, 1, hitPosition);

            return true;
        }

        private void DealDamage(Vector3 hitPosition)
        {
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

        private async Task SendLightningStrike(Vector3 hitPosition, IEnumerable<ITransportConnection> players, NetId lwcNetId)
        {
            lock (this)
                _currentLighningCount++;

            await Task.Delay(PRE_STRIKE_DELAY);
            _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, players, hitPosition);
            await Task.Delay(POST_STRIKE_DELAY);

            lock (this)
                _currentLighningCount--;

            if (_currentLighningCount == 0)
            {
                _weatherProvider.RestoreWeather();
                LWCNetId = NetId.INVALID;
            }
        }
    }
}