using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightPossible : IBufferElementData, IEnableableComponent
    {
        public Entity Source;
        public float3 SourcePosition;
    }
}