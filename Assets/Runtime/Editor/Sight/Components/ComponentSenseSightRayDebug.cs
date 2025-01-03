using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECSPerception.Editor.Sight
{
    public struct ComponentSenseSightRayDebug : IComponentData
    {
        public Color ColorSuccess;
        public Color ColorFailure;
        public Color ColorNeutral;

        public float3 SizeOctahedronSmall;
        public float3 SizeOctahedronStandard;

        public static ComponentSenseSightRayDebug Default => new()
        {
            ColorSuccess = Color.green,
            ColorFailure = Color.red,
            ColorNeutral = Color.yellow,

            SizeOctahedronSmall = new float3(0.1f, 0.2f, 0.1f),
            SizeOctahedronStandard = new float3(0.3f, 0.6f, 0.3f),
        };
    }
}