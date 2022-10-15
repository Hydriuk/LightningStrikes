#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using LightningStrikes.API;
using SDG.Unturned;
using System.Reflection;
using UnityEngine;

namespace LightningStrikes.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class WeatherProvider : IWeatherProvider
    {
        private readonly static FieldInfo _weatherForecastTimerGetter = typeof(LightingManager).GetField("scheduledWeatherForecastTimer", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly static FieldInfo _weatherActiveTimerGetter = typeof(LightingManager).GetField("scheduledWeatherActiveTimer", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly static FieldInfo _scheduledWeatherRefGetter = typeof(LightingManager).GetField("scheduledWeatherRef", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly static FieldInfo _activeCustomWeatherGetter = typeof(LevelLighting).GetField("activeCustomWeather", BindingFlags.NonPublic | BindingFlags.Static);

        private WeatherAssetBase _currentWeather;
        private WeatherAssetBase _scheduledWeather;
        private float _scheduledWeatherDuration;
        private float _weatherTimeLeft;
        private bool _rescheduleWeather = false;

        public WeatherProvider()
        {
            _currentWeather = LevelLighting.GetActiveWeatherAsset();
            _scheduledWeather = ((AssetReference<WeatherAssetBase>)_scheduledWeatherRefGetter.GetValue(null)).get();
        }

        /// <summary>
        /// Sets a weather that has lightnings
        /// </summary>
        /// <returns> NetId of the active Lightning Weather Component </returns>
        public NetId SetLightningWeather()
        {
            object weather = _activeCustomWeatherGetter.GetValue(null);

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
            else
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

            _currentWeather = LevelLighting.GetActiveWeatherAsset();

            _weatherTimeLeft = (float)_weatherForecastTimerGetter.GetValue(null);

            _scheduledWeather = ((AssetReference<WeatherAssetBase>)_scheduledWeatherRefGetter.GetValue(null)).get();

            _scheduledWeatherDuration = (float)_weatherActiveTimerGetter.GetValue(null);
        }

        /// <summary>
        /// Changes the current active weather for one that has lightnings
        /// </summary>
        /// <returns> The new weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object SetCustomLightningWeather()
        {
            // Heavy rain weather
            AssetReference<WeatherAssetBase>.TryParse("6c850687bdb947a689fa8de8a8d99afb", out AssetReference<WeatherAssetBase> assetRef);

            WeatherAssetBase weatherAsset = assetRef.get();
            LightingManager.ActivatePerpetualWeather(weatherAsset);

            return _activeCustomWeatherGetter.GetValue(null);
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