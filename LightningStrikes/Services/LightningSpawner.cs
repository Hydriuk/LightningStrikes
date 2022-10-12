using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LightningStrikes.Configuration;
using Random = UnityEngine.Random;

namespace LightningStrikes.Services
{
    public class LightningSpawner : MonoBehaviour
    {
        private const int PRE_STRIKE_DELAY = 1;
        private const int POST_STRIKE_DELAY = 1000;

        private ClientInstanceMethod<Vector3> _sendLightningStrike;

        private float _lightningRangeRadius = -1f;
        private static int _currentLighningCount = 0;
        private static NetId LWCNetId = NetId.INVALID;

        public LightningSpawner()
        {
            _sendLightningStrike = (ClientInstanceMethod<Vector3>)typeof(LightningWeatherComponent)
                .GetField("SendLightningStrike", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);

            _sendLightningStrike = (ClientInstanceMethod<Vector3>)typeof(LightningWeatherComponent)
                .GetField("DoExplosionDamage", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
        }

        public void SendStrike(EventConf eventConf, Vector3 position)
        {
            switch (eventConf.StrikeType)
            {
                case Models.EStrikeType.SingleStrike:
                    Strike(position, dealDamage: eventConf.Damage);
                    break;
                case Models.EStrikeType.StrikeRing:
                    StrikeRing(position, eventConf.Amount, eventConf.Radius, eventConf.MinDelay, eventConf.MaxDelay, eventConf.Damage);
                    break;
                default:
                    Strike(position);
                    break;
            }
        }

        public bool Strike(Vector3 hitPosition, bool dealDamage = false)
        {
            return SendLightningStrike(hitPosition, dealDamage);
            //if (player == null)
            //else
            //    return SendLightningStrike(hitPosition, player, dealDamage);
        }

        public void StrikeRing(Vector3 center, int count, float radius, int minDelay = 50, int maxDelay = 50, bool dealDamage = false)
        {
            Vector3[] strikePostions = new Vector3[count];
            float angle = 2*Mathf.PI / count;

            for (int i = 0; i < count; i++)
            {
                strikePostions[i] = new Vector3(
                    center.x + radius * Mathf.Cos(angle*i),
                    center.y,
                    center.z + radius * Mathf.Sin(angle*i)
                );
            }

            Task.Run(async () =>
            {

                foreach (var strikePostion in strikePostions)
                {
                    Console.WriteLine("Sending strike");
                    SendLightningStrike(strikePostion, dealDamage);
                    await Task.Delay(Random.Range(minDelay, maxDelay));
                }
            });
        }

        //public bool StrikeCircle(Vector3 center, int count, float radius, int minDelay = 0, int maxDelay = 0)
        //{
        //    Vector3[] strikePostions = new Vector3[count];

        //    for (int i = 0; i < count; i++)
        //    {
        //        strikePostions[i] =
        //    }

        //    foreach (var strikePostion in strikePostions)
        //    {
        //        SendLightningStrike(strikePostion);
        //    }
        //}


        private bool SendLightningStrike(Vector3 hitPosition, SteamPlayer player, bool dealDamage = false)
        {
            if(LWCNetId == NetId.INVALID)
                LWCNetId = LightningStrikes.Instance.WeatherProvider.SetLightningWeather();

            if (LWCNetId == NetId.INVALID)
                return false;

            _ = SendLightningStrike(hitPosition, player.transportConnection, LWCNetId);

            if (dealDamage)
                StartCoroutine(DealDamage(hitPosition));

            return true;
        }

        private bool SendLightningStrike(Vector3 hitPosition, bool dealDamage = false)
        {
            if (LWCNetId == NetId.INVALID)
                LWCNetId = LightningStrikes.Instance.WeatherProvider.SetLightningWeather();

            if (LWCNetId == NetId.INVALID)
                return false;

            if (_lightningRangeRadius > 0f)
                _ = SendLightningStrike(hitPosition, Provider.EnumerateClients_WithinSphere(hitPosition, _lightningRangeRadius), LWCNetId);
            else
                _ = SendLightningStrike(hitPosition, Provider.EnumerateClients(), LWCNetId);

            if (dealDamage)
                StartCoroutine(DealDamage(hitPosition));

            return true;
        }

        private IEnumerator<object> DealDamage(Vector3 hitPosition)
        {
            yield return new WaitForSeconds(1f);
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

        #region internal sendLightningStrike
        private async Task SendLightningStrike(Vector3 hitPosition, IEnumerable<ITransportConnection> players, NetId lwcNetId)
        {
            _currentLighningCount++;

            await Task.Delay(PRE_STRIKE_DELAY);
            _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, players, hitPosition);
            await Task.Delay(POST_STRIKE_DELAY);

            _currentLighningCount--;

            if(_currentLighningCount == 0)
            {
                LightningStrikes.Instance.WeatherProvider.RestoreWeather();
                LWCNetId = NetId.INVALID;
            }
        }

        private async Task SendLightningStrike(Vector3 hitPosition, ITransportConnection player, NetId lwcNetId)
        {
            _currentLighningCount++;

            await Task.Delay(PRE_STRIKE_DELAY);
            _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, player, hitPosition);
            await Task.Delay(POST_STRIKE_DELAY);

            _currentLighningCount--;

            if (_currentLighningCount == 0)
            {
                LightningStrikes.Instance.WeatherProvider.RestoreWeather();
                LWCNetId = NetId.INVALID;
            }
        }
        #endregion
    }
}
