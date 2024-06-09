using LightningStrikes.API;
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
        public IStrikePositionProvider StrikePositionProvider;


        public LightningStrikes()
        {
            Instance = this;
        }

        protected override void Load()
        {
            ThreadManager = new ThreadManager();
            WeatherProvider = new WeatherProvider(ThreadManager);
            StrikePositionProvider = new StrikePositionProvider();
            LightningSpawner = new LightningSpawner(WeatherProvider, ThreadManager);
        }

        protected override void Unload()
        {
            WeatherProvider.Dispose();
        }
    }
}