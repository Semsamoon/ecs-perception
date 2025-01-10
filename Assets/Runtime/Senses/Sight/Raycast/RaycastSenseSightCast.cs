using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct RaycastSenseSightCast
    {
        public Entity Receiver;
        public float3 ReceiverPosition;
        public Entity Source;
        public float3 SourcePosition;
        public float NearClipRadiusSquared;

        public RaycastSenseSightCast(in BufferSenseSightCastExecute castExecute)
        {
            Receiver = castExecute.Receiver;
            ReceiverPosition = castExecute.ReceiverPosition;
            Source = castExecute.Source;
            SourcePosition = castExecute.SourcePosition;
            NearClipRadiusSquared = castExecute.NearClipRadiusSquared;
        }
    }
}