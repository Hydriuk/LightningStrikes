using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 
        /// </summary>
        /// <returns> The active weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object GetActiveWeather()
        {
            return typeof(LevelLighting)
                .GetField("activeCustomWeather", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
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
    }
}
