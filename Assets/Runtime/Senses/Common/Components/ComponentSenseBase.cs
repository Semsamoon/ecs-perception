using Unity.Entities;

namespace PerceptionECS
{
    public struct ComponentSenseBase : IComponentData
    {
        public Entity Entity;
    }
}