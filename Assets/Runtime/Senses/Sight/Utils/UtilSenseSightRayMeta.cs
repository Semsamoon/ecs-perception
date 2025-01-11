using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct UtilSenseSightRayMeta
    {
        public DynamicBuffer<BufferSenseSightActive> Actives;
        public DynamicBuffer<BufferSenseSightMemory> Memories;
        public float RememberTime;
    }
}