using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseSourceDestroy : IComponentData
    {
        public Entity Entity;
    }
}