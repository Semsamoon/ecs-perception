using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECSPerception.Editor.Sight
{
    public struct ComponentSenseSightDebug : IComponentData
    {
        [Header("Rays")]
        public Color RaySuccessColor;
        public Color RayFailureColor;
        public Color RayNeutralColor;

        public float OctahedronSmallScale;
        public float OctahedronStandardScale;

        public float3 SizeOctahedronSmall => new float3(0.5f, 1, 0.5f) * OctahedronSmallScale;
        public float3 SizeOctahedronStandard => new float3(0.5f, 1, 0.5f) * OctahedronStandardScale;

        [Header("Receivers")]
        public Color ReceiverNearClipColor;
        public Color ReceiverViewColor;
        public Color ReceiverLoseColor;

        public float CurveStep;

        public static ComponentSenseSightDebug Default => new()
        {
            RaySuccessColor = Color.green,
            RayFailureColor = Color.red,
            RayNeutralColor = Color.yellow,

            OctahedronSmallScale = 0.2f,
            OctahedronStandardScale = 0.6f,

            ReceiverNearClipColor = Color.gray,
            ReceiverViewColor = Color.green,
            ReceiverLoseColor = Color.red,

            CurveStep = 0.02f,
        };
    }
}