using Unity.Entities;

namespace PerceptionECS
{
    public struct ComponentSenseReceiver : IComponentData
    {
        public Entity Owner;
        public Entity Transform;
    }
}