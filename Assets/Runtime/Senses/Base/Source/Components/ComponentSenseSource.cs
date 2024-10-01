using Unity.Entities;

namespace PerceptionECS
{
    public struct ComponentSenseSource : IComponentData
    {
        public Entity Owner;
        public Entity Transform;
    }
}