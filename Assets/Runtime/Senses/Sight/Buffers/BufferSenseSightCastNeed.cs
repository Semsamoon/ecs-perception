using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightCastNeed : IBufferElementData, IEnableableComponent
    {
        public Entity Source;
        public float3 SourcePosition;
    }
}