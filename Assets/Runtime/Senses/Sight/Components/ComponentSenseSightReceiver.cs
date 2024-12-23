using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightReceiver : IComponentData
    {
        public float ViewAngleCos;
        public float ViewRadiusSquared;
        public float LoseRadiusSquared;
        public float NearClipRadiusSquared;
        public float3 Offset;
        public float RememberTime;
    }
}