using Unity.Entities;
using Unity.Transforms;

namespace PerceptionECS
{
    public struct ComponentSenseRemember : IComponentData
    {
        public LocalToWorld SourceTransform;
        public float Timer;
    }
}