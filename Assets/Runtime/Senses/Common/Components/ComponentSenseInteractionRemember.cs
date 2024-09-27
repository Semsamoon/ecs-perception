using Unity.Entities;
using Unity.Mathematics;

namespace PerceptionECS
{
    public struct ComponentSenseInteractionRemember : IComponentData
    {
        public float3 SourcePosition;
        public float Duration;

        public void Deconstruct(out float3 sourcePosition, out float duration)
        {
            sourcePosition = SourcePosition;
            duration = Duration;
        }
    }
}