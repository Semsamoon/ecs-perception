using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception
{
    public struct ComponentSenseContactRemember : IComponentData
    {
        public LocalToWorld SourceTransform;
        public float Timer;
    }
}