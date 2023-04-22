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
    /// <summary>
    /// The <see cref="LightningWeatherComponent"/> is the client side object that creates the lightning. The client needs an active instance of this object to generate a lightning.
    /// <see cref="WeatherProvider"/> changes the current weather to one that has a <see cref="LightningWeatherComponent"/> instanciated with it.
    /// </summary>
    public class WeatherProvider : IWeatherProvider
    {
        private readonly static FieldInfo _weatherForecastTimerGetter = typeof(LightingManager).GetField("scheduledWeatherForecastTimer", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly static FieldInfo _activeCustomWeatherGetter = typeof(LevelLighting).GetField("activeCustomWeather", BindingFlags.NonPublic | BindingFlags.Static);

        private WeatherAssetBase _currentWeather;
        private float _weatherTimeLeft;
        private bool _rescheduleWeather = false;

        public WeatherProvider()
        {
            _currentWeather = LevelLighting.GetActiveWeatherAsset();
        }

        /// <summary>
        /// Sets a weather that has lightnings
        /// </summary>
        /// <returns> NetId of the active Lightning Weather Component </returns>
        public NetId SetLightningWeather()
        {
            object weather = _activeCustomWeatherGetter.GetValue(null);

            NetId lwcNetId = GetLighningNetId(weather);

            if (lwcNetId == NetId.INVALID)
            {
                // Save weather state only if not already saved
                if (!_rescheduleWeather)
                {
                    SaveWeatherState();
                }

                weather = SetCustomLightningWeather();

                lwcNetId = GetLighningNetId(weather);

                _rescheduleWeather = true;
            }

            return lwcNetId;
        }

        /// <summary>
        /// Restore the weather state if weather was changed for a lightning one
        /// </summary>
        public void RestoreWeather()
        {
            if(_rescheduleWeather)
            {
                if(_currentWeather == null)
                {
                    LightingManager.DisableWeather();
                }
                else
                {
                    LightingManager.ForecastWeatherImmediately(_currentWeather);
                }

                LightingManager.ResetScheduledWeather();

                _weatherForecastTimerGetter.SetValue(null, _weatherTimeLeft);
                _rescheduleWeather = false;
            }
        }

        /// <summary>
        /// Saves the current weather state
        /// </summary>
        private void SaveWeatherState()
        {
            _currentWeather = LevelLighting.GetActiveWeatherAsset();
            _weatherTimeLeft = (float)_weatherForecastTimerGetter.GetValue(null);
        }

        /// <summary>
        /// Changes the current active weather for one that has lightnings
        /// </summary>
        /// <returns> The new weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object SetCustomLightningWeather()
        {
            // Heavy rain weather
            AssetReference<WeatherAssetBase>.TryParse("6c850687bdb947a689fa8de8a8d99afb", out AssetReference<WeatherAssetBase> assetRef);

            WeatherAssetBase weatherAsset = assetRef.Find();
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
            if (weather == null)
                return NetId.INVALID;

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