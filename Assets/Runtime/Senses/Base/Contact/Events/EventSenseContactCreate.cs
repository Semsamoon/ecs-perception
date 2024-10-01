using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseContactCreate : IComponentData
    {
        public Entity Entity;
        public Entity Receiver;
        public Entity Source;
    }
}