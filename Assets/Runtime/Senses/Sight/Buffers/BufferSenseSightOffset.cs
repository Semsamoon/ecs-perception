using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightOffset : IBufferElementData
    {
        public float3 Offset;
    }
}