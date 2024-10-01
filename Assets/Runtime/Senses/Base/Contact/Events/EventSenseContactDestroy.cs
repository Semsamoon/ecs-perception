using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseContactDestroy : IComponentData
    {
        public Entity Entity;
    }
}