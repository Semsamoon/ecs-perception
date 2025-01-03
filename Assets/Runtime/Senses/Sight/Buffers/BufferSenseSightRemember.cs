using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightRemember : IBufferElementData
    {
        public Entity Source;
        public float3 SourcePosition;
        public float Timer;
    }
}