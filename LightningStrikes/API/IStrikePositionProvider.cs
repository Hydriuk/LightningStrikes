#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LightningStrikes.API
{
#if OPENMOD
    [Service]
#endif
    public interface IStrikePositionProvider
    {
        Vector3 GetLookedPosition(Player player);

        Vector3[] GetPositions(Vector3 center, int count, float radius, bool random = false, bool circle = false, bool ground = false);
    }
}
