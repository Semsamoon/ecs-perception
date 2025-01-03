using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightActive : IBufferElementData
    {
        public Entity Source;
        public float3 SourcePosition;
    }
}