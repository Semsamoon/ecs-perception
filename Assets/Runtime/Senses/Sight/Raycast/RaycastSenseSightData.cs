using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct RaycastSenseSightData
    {
        public float3 ReceiverPosition;
        public Entity Receiver;
        public float3 SourcePosition;
        public Entity Source;

        public RaycastSenseSightData(float3 receiverPosition, Entity receiver, float3 sourcePosition, Entity source)
        {
            ReceiverPosition = receiverPosition;
            Receiver = receiver;
            SourcePosition = sourcePosition;
            Source = source;
        }
    }
}