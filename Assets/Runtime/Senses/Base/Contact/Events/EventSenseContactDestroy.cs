using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseContactDestroy : IComponentData
    {
        public Entity Entity;
    }
}