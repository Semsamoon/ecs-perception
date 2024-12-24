using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightCastExtraTwo : IComponentData
    {
        public float3 Offset1;
        public float3 Offset2;
    }
}