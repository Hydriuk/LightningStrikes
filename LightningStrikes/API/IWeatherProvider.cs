#if OPENMOD
using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace LightningStrikes.API
{
#if OPENMOD
    [Service]
#endif
    public interface IWeatherProvider : IDisposable
    {
        Task<NetId> SetLightningWeather();
        UniTask RestoreWeather();
    }
}