using Unity.Entities;

namespace ECSPerception
{
    public struct ComponentSenseReceiver : IComponentData
    {
        public Entity Owner;
        public Entity Transform;
    }
}