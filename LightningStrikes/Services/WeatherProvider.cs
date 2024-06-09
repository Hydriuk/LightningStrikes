#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using LightningStrikes.API;
using SDG.Unturned;
using System;
using System.Reflection;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

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
        private const int POST_STRIKE_DELAY = 2000;

        private readonly static FieldInfo _weatherForecastTimerGetter = typeof(LightingManager).GetField("scheduledWeatherForecastTimer", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly static FieldInfo _activeCustomWeatherGetter = typeof(LevelLighting).GetField("activeCustomWeather", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly Timer _timer;
        private readonly WeatherAssetBase _lightningAsset;

        private readonly IThreadManager _threadManager;

        public WeatherProvider(IThreadManager threadManager)
        {
            _threadManager = threadManager;

            _timer = new Timer(RestoreWeather);

            AssetReference<WeatherAssetBase>.TryParse("68f4c2961e224acab4d3b70ebba55149", out AssetReference<WeatherAssetBase> lightningAssetRef);
            _lightningAsset = lightningAssetRef.Find();
        }

        public void Dispose()
        {
            _timer.Dispose();

            RestoreWeather();
        }

        /// <summary>
        /// Sets a weather that has lightnings
        /// </summary>
        /// <returns> NetId of the active Lightning Weather Component </returns>
        public NetId SetLightningWeather()
        {
            var weather = SetCustomLightningWeather();
            var lwcNetId = GetLighningNetId(weather);

            _timer.Change(POST_STRIKE_DELAY, Timeout.Infinite);

            return lwcNetId;
        }

        private void RestoreWeather(object state) => RestoreWeather();
        /// <summary>
        /// Restore the weather state if weather was changed for a lightning one
        /// </summary>
        public void RestoreWeather()
        {
            _threadManager.RunOnMainThread(LightingManager.ResetScheduledWeather);
        }

        /// <summary>
        /// Changes the current active weather for one that has lightnings
        /// </summary>
        /// <returns> The new weather object of type <see cref="LevelLighting.CustomWeatherInstance"/></returns>
        private object SetCustomLightningWeather()
        {
            LightingManager.ActivatePerpetualWeather(_lightningAsset);

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