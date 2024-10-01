using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseReceiverCreate : IComponentData
    {
        public Entity Entity;
        public Entity Owner;
        public Entity Transform;
        public float RememberTime;
    }
}