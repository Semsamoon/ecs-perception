using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseSourceDestroy : IComponentData
    {
        public Entity Entity;
    }
}