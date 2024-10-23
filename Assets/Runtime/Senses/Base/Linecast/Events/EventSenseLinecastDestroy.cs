using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseLinecastDestroy : IComponentData
    {
        public Entity Entity;
    }
}