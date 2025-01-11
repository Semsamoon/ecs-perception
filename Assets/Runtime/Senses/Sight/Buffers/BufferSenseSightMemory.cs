using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightMemory : IBufferElementData
    {
        public Entity Source;
        public float3 SourcePosition;
        public float Timer;
    }
}