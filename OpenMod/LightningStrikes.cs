using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
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
