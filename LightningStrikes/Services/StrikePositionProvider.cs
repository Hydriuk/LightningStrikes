using LightningStrikes.API;
#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LightningStrikes.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class StrikePositionProvider : IStrikePositionProvider
    {
        private const float PI_2 = 2 * Mathf.PI;

        public Vector3 GetLookedPosition(Player player)
        {
            // Raycast target player's look
            Physics.Raycast(
                player.look.aim.position,
                player.look.aim.forward,
                out RaycastHit hit,
                1000f,
                RayMasks.BLOCK_COLLISION - RayMasks.SKY,
                QueryTriggerInteraction.UseGlobal
            );

            return hit.point;
        }

        public Vector3[] GetPositions(Vector3 center, int count, float radius, bool random = false, bool circle = false, bool ground = false)
        {
            Vector3[] strikePostions = new Vector3[count];
            float angle = PI_2 / count;

            for (int i = 0; i < count; i++)
            {
                float cosX;
                float sinY;

                if (circle)
                {
                    strikePostions[i] = center + Random.onUnitSphere * radius;
                    //strikePostions[i].y = LevelGround.getHeight(strikePostions[i]);
                    //continue;
                }
                else
                {
                    if (random)
                    {
                        var rn = Random.value;
                        cosX = Mathf.Cos(PI_2 * rn);
                        sinY = Mathf.Sin(PI_2 * rn);
                    }
                    else
                    {
                        cosX = Mathf.Cos(angle * i);
                        sinY = Mathf.Sin(angle * i);
                    }

                    strikePostions[i] = new Vector3(
                        center.x + radius * cosX,
                        center.y,
                        center.z + radius * sinY
                    );
                }

                if (ground)
                {
                    strikePostions[i].y = LevelGround.getHeight(strikePostions[i]);
                }
                else
                {
                    Physics.Raycast(
                        new Vector3(strikePostions[i].x, Level.HEIGHT, strikePostions[i].z),
                        Vector3.down,
                        out var hitInfo,
                        Level.HEIGHT * 2f,
                        (int)(ERayMask.RESOURCE | ERayMask.LARGE | ERayMask.MEDIUM | ERayMask.ENVIRONMENT | ERayMask.GROUND | ERayMask.VEHICLE | ERayMask.BARRICADE | ERayMask.STRUCTURE)
                    );
                    strikePostions[i].y = hitInfo.point.y;
                }
            }

            return strikePostions;
        }

    }
}
