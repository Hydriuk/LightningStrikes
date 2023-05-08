using LightningStrikes.API;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.NetTransport;
using SDG.Unturned;
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
        private const int PRE_STRIKE_DELAY = 30;
        private const int POST_STRIKE_DELAY = 2000;

        private readonly static ClientInstanceMethod<Vector3> _sendLightningStrike = ClientInstanceMethod<Vector3>.Get(typeof(LightningWeatherComponent), "ReceiveLightningStrike");

        private readonly IWeatherProvider _weatherProvider;
        private readonly IThreadManager _threadManager;

        private NetId LWCNetId = NetId.INVALID;
        private Timer _timer;

        public LightningSpawner(IWeatherProvider weatherProvider, IThreadManager threadManager)
        {
            _weatherProvider = weatherProvider;
            _threadManager = threadManager;
            _timer = new Timer(ResetWeather);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void Strike(Vector3 hitPosition, bool dealDamage = false)
        {
            SendLightningStrike(hitPosition, dealDamage);
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

        private void SendLightningStrike(Vector3 hitPosition, bool dealDamage = false)
        {
            if (LWCNetId == NetId.INVALID)
                LWCNetId = _weatherProvider.SetLightningWeather();

            if (LWCNetId == NetId.INVALID)
                return;

            _ = SendLightningStrike(hitPosition, LWCNetId);

            if (dealDamage)
                _threadManager.Execute(DealDamage, 1, hitPosition);
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

        private async Task SendLightningStrike(Vector3 hitPosition,NetId lwcNetId)
        {
            await Task.Delay(PRE_STRIKE_DELAY);
            _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, Provider.GatherClientConnections(), hitPosition);

            _timer.Change(POST_STRIKE_DELAY, Timeout.Infinite);
        }

        private void ResetWeather(object state)
        {
            System.Console.WriteLine("Try restoring");
            _weatherProvider.RestoreWeather();
            LWCNetId = NetId.INVALID;
        }
    }
}