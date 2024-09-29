using Unity.Entities;
using Unity.Transforms;

namespace PerceptionECS
{
    public struct ComponentSenseContactRemember : IComponentData
    {
        public LocalToWorld SourceTransform;
        public float Timer;
    }
}