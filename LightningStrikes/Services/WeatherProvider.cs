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
    public class WeatherProvider : MonoBehaviour
    {
        private WeatherAssetBase _currentWeather;
        private WeatherAssetBase _scheduledWeather;
        private float _scheduledWeatherDuration;
        private float _weatherTimeLeft;
        private bool _rescheduleWeather = false;

        /// <summary>
        /// Sets a weather that has lightnings
        /// </summary>
        /// <returns> NetId of the active Lightning Weather Component </returns>
        public NetId SetLightningWeather()
        {
            object weather = GetActiveWeather();

            NetId lwcNetId;
            if (weather != null)
            {
                lwcNetId = GetLighningNetId(weather);

                if (lwcNetId == NetId.INVALID)
                {
                    SaveWeatherState(weather);

                    weather = SetCustomLightningWeather();

                    lwcNetId = GetLighningNetId(weather);
                }
            }
            else
            {
                weather = SetCustomLightningWeather();

                lwcNetId = GetLighningNetId(weather);
            }

            return lwcNetId;
        }

        /// <summary>
        /// Restore the weather state if weather was changed for a lightning one
        /// </summary>
        public void RestoreWeather()
        {
            if (_rescheduleWeather)
            {
                LightingManager.ActivatePerpetualWeather(_currentWeather);

                LightingManager.SetScheduledWeather(_scheduledWeather, _weatherTimeLeft, _scheduledWeatherDuration);
                _rescheduleWeather = false;
            }
            else if (!_rescheduleWeather)
            {
                LightingManager.ResetScheduledWeather();
            }
        }

        /// <summary>
        /// Saves the current weather state
        /// </summary>
        /// <param name="weather"> The current active <see cref="LevelLighting.CustomWeatherInstance"/> </param>
        private void SaveWeatherState(object weather)
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
        }

        /// <summary>
        /// Get the current active weather
        /// </summary>
        /// <returns> The active weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object GetActiveWeather()
        {
            return typeof(LevelLighting)
                .GetField("activeCustomWeather", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
        }



        /// <summary>
        /// Changes the current active weather for one that has lightnings
        /// </summary>
        /// <returns> The new weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object SetCustomLightningWeather()
        {
            AssetReference<WeatherAssetBase>.TryParse("6c850687bdb947a689fa8de8a8d99afb", out AssetReference<WeatherAssetBase> assetRef);

            WeatherAssetBase weatherAsset = assetRef.get();
            LightingManager.ActivatePerpetualWeather(weatherAsset);
            return GetActiveWeather();
        }

        /// <summary>
        /// Gets the active Lightning Weather Component NetId
        /// </summary>
        /// <param name="weather"> The current active <see cref="LevelLighting.CustomWeatherInstance"/> </param>
        /// <returns> NetId of the active Lightning Weather Component </returns>
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
    }
}
