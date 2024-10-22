using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseReceiverDestroy : IComponentData
    {
        public Entity Entity;
    }
}