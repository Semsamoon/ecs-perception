using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightSource : IComponentData
    {
        public float3 Offset;
    }
}