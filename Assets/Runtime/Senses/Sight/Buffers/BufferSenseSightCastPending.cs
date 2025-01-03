using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightCastPending : IBufferElementData, IEnableableComponent
    {
        public RaycastSenseSightCast Raycast;
    }
}