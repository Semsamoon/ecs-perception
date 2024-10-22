using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseSourceCreate : IComponentData
    {
        public Entity Entity;
        public Entity Owner;
        public Entity Transform;
    }
}