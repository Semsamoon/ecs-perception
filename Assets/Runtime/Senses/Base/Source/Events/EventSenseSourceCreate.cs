using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseSourceCreate : IComponentData
    {
        public Entity Entity;
        public Entity Owner;
        public Entity Transform;
    }
}