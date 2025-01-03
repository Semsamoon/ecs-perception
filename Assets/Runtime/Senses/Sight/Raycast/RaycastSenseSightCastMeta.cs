using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct RaycastSenseSightCastMeta
    {
        public DynamicBuffer<BufferSenseSightActive> Actives;
        public DynamicBuffer<BufferSenseSightRemember> Remembers;
        public float RememberTime;
    }
}