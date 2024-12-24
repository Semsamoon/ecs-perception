using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightCast : IBufferElementData
    {
        public RaycastSenseSightData Raycast;

        public BufferSenseSightCast(RaycastSenseSightData raycast)
        {
            Raycast = raycast;
        }
    }
}