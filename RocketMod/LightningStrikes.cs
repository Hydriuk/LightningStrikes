﻿using LightningStrikes.API;
using LightningStrikes.RocketMod.Services;
using LightningStrikes.Services;
using Rocket.Core.Plugins;

namespace LightningStrikes.RocketMod
{
    public class LightningStrikes : RocketPlugin
    {
        public static LightningStrikes Instance;

        public ILightningSpawner LightningSpawner;
        public IWeatherProvider WeatherProvider;
        public IThreadManager ThreadManager;


        public LightningStrikes()
        {
            Instance = this;
        }

        protected override void Load()
        {
            WeatherProvider = new WeatherProvider();
            ThreadManager = new ThreadManager();
            LightningSpawner = new LightningSpawner(WeatherProvider, ThreadManager);
        }
    }
}
