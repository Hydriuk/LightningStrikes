#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace LightningStrikes.API
{
#if OPENMOD
    [Service]
#endif
    public interface ILightningSpawner
    {
        bool Strike(Vector3 hitPosition, bool dealDamage = false);
        void StrikeRing(Vector3[] strikePostions, int minDelay = 50, int maxDelay = 50, bool dealDamage = false);
    }
}