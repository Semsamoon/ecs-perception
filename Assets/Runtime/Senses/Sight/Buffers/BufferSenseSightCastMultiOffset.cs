using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightCastMultiOffset : IBufferElementData
    {
        public float3 Offset;
    }
}