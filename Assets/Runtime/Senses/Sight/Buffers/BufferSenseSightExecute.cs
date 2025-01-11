using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightExecute : IBufferElementData, IEnableableComponent
    {
        public Entity Receiver;
        public float3 ReceiverPosition;
        public Entity Source;
        public float3 SourcePosition;
        public float NearClipRadiusSquared;
    }
}