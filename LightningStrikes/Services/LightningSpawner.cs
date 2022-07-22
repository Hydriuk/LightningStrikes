using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LightningStrikes.Services
{
    public class LightningSpawner : MonoBehaviour
    {
        //private HashSet<WeatherAssetBase> _weatherAssetBases;

        private ClientInstanceMethod<Vector3> _sendLightningStrike;

        private float _lightningRangeRadius = -1;

        private static int _currentLighningCount;



        private static WeatherAssetBase _currentWeather;
        private static WeatherAssetBase _scheduledWeather;
        private static float _scheduledWeatherDuration;
        private static float _weatherTimeLeft;
        private static bool _rescheduleWeather = false;

        public LightningSpawner()
        {
            _sendLightningStrike = (ClientInstanceMethod<Vector3>)typeof(LightningWeatherComponent)
                .GetField("SendLightningStrike", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> The active weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object GetActiveWeather()
        {
            return typeof(LevelLighting)
                .GetField("activeCustomWeather", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
        }

        private NetId GetLighningNetId(object weather)
        {
            GameObject weatherGameObject = (GameObject)weather
                .GetType()
                .GetField("gameObject")
                .GetValue(weather);

            LightningWeatherComponent weatherLightning = weatherGameObject.GetComponent<LightningWeatherComponent>();

            if (weatherLightning == null)
                return NetId.INVALID;

            return weatherLightning.GetNetId();
        }


        public bool SendLightningStrike(Vector3 hitPosition, SteamPlayer player)
        {
            object weather = GetActiveWeather();

            if (weather == null)
                return false;

            NetId lwcNetId = GetLighningNetId(weather);

            if (lwcNetId == NetId.INVALID)
                return false;

            _ = SendLightningStrike(hitPosition, player.transportConnection, lwcNetId);

            return true;
        }

        public void SendLightningStrike(Vector3 hitPosition)
        {
            object weather = GetActiveWeather();

            NetId lwcNetId;
            if (weather != null)
            {
                lwcNetId = GetLighningNetId(weather);

                if(lwcNetId == NetId.INVALID)
                {
                    _rescheduleWeather = true;
                    _currentWeather = (WeatherAssetBase)weather.GetType()
                        .GetField("asset")
                        .GetValue(weather);

                    _weatherTimeLeft = (float)typeof(LightingManager)
                        .GetField("scheduledWeatherForecastTimer", BindingFlags.NonPublic | BindingFlags.Static)
                        .GetValue(null);

                    _scheduledWeather = ((AssetReference<WeatherAssetBase>)typeof(LightingManager)
                        .GetField("scheduledWeatherRef", BindingFlags.NonPublic | BindingFlags.Static)
                        .GetValue(null))
                        .get();

                    _scheduledWeatherDuration = (float)typeof(LightingManager)
                        .GetField("scheduledWeatherForecastTimer", BindingFlags.NonPublic | BindingFlags.Static)
                        .GetValue(null);

                    weather = SetCustomLighningWeather();

                    lwcNetId = GetLighningNetId(weather);
                }
            }
            else
            {
                weather = SetCustomLighningWeather();

                lwcNetId = GetLighningNetId(weather);
            }

            if (_lightningRangeRadius > 0)
                _ = SendLightningStrike(hitPosition, Provider.EnumerateClients_WithinSphere(hitPosition, _lightningRangeRadius), lwcNetId);
            else
                _ = SendLightningStrike(hitPosition, Provider.EnumerateClients(), lwcNetId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> The new weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object SetCustomLighningWeather()
        {
            AssetReference<WeatherAssetBase>.TryParse("6c850687bdb947a689fa8de8a8d99afb", out AssetReference<WeatherAssetBase> assetRef);

            WeatherAssetBase weatherAsset = assetRef.get();
            LightingManager.ActivatePerpetualWeather(weatherAsset);
            return GetActiveWeather();
        }

        private async Task SendLightningStrike(Vector3 hitPosition, IEnumerable<ITransportConnection> players, NetId lwcNetId)
        {
            _currentLighningCount++;

            await Task.Delay(1);
            _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, players, hitPosition);
            await Task.Delay(2000);

            _currentLighningCount--;

            if (_rescheduleWeather && _currentLighningCount == 0)
            {
                LightingManager.ActivatePerpetualWeather(_currentWeather);

                LightingManager.SetScheduledWeather(_scheduledWeather, _weatherTimeLeft, _scheduledWeatherDuration);
                _rescheduleWeather = false;
            }
            else if (!_rescheduleWeather && _currentLighningCount == 0)
            {
                LightingManager.ResetScheduledWeather();
            }
        }

        private async Task SendLightningStrike(Vector3 hitPosition, ITransportConnection player, NetId lwcNetId)
        {
            _currentLighningCount++;

            await Task.Delay(1);
            _sendLightningStrike.Invoke(lwcNetId, ENetReliability.Reliable, player, hitPosition);
            await Task.Delay(2000);

            _currentLighningCount--;

            if(_rescheduleWeather && _currentLighningCount == 0)
            {
                LightingManager.ActivatePerpetualWeather(_currentWeather);

                LightingManager.SetScheduledWeather(_scheduledWeather, _weatherTimeLeft, _scheduledWeatherDuration);
                _rescheduleWeather = false;
            }
            else if(!_rescheduleWeather && _currentLighningCount == 0)
            {
                LightingManager.ResetScheduledWeather();
            }
        }
    }
}
