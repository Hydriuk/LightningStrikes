#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace LightningStrikes.API
{
#if OPENMOD
    [Service]
#endif
    public interface IWeatherProvider
    {
        NetId SetLightningWeather();
        void RestoreWeather();
    }
}