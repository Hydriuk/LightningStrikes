using HarmonyLib;
using LightningStrikes.Services;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHandler = LightningStrikes.Services.EventHandler;

namespace LightningStrikes
{
    public class LightningStrikes : RocketPlugin<Configuration>
    {
        public const string WORKSHOP_ID = "";

        public static LightningStrikes Instance;

        public LightningSpawner LightningSpawner;
        public WeatherProvider WeatherProvider;
        public EventHandler EventHandler;


        public LightningStrikes()
        {
            Instance = this;
        }

        protected override void Load()
        {
            base.Load();

            LightningSpawner = this.gameObject.AddComponent<LightningSpawner>();
            WeatherProvider = this.gameObject.AddComponent<WeatherProvider>();
            EventHandler = this.gameObject.AddComponent<EventHandler>();

            Harmony harmony = new Harmony("Hydriuk.Unturned.LightningStrikes");
            harmony.PatchAll();

            //Provider.getServerWorkshopFileIDs();
        }
    }
}
