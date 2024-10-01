using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseReceiverDestroy : IComponentData
    {
        public Entity Entity;
    }
}