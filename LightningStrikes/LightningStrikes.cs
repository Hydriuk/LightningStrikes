using LightningStrikes.Services;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningStrikes
{
    public class LightningStrikes : RocketPlugin
    {
        public const string WORKSHOP_ID = "";

        public static LightningSpawner LightningSpawner;

        protected override void Load()
        {
            base.Load();

            LightningSpawner = this.gameObject.AddComponent<LightningSpawner>();
        }
    }
}
