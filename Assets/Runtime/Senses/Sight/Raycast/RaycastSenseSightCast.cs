using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception.Sight
{
    public struct RaycastSenseSightCast
    {
        public float3 ReceiverPosition;
        public Entity Receiver;
        public float3 SourcePosition;
        public Entity Source;
    }
}