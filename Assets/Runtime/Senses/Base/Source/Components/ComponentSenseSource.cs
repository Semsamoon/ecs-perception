using Unity.Entities;

namespace ECSPerception
{
    public struct ComponentSenseSource : IComponentData
    {
        public Entity Owner;
        public Entity Transform;
    }
}