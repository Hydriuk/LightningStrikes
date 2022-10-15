using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("LightningStrikes", DisplayName = "LightningStrikes")]

namespace LightningStrikes.OpenMod
{
    public class LightningStrikes : OpenModUnturnedPlugin
    {
        public LightningStrikes(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}