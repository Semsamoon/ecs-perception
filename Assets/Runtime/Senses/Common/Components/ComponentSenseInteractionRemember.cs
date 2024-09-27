using Unity.Entities;
using Unity.Mathematics;

namespace PerceptionECS
{
    public struct ComponentSenseInteractionRemember : IComponentData
    {
        public float3 SourcePosition;
        public float Timer;

        public void Deconstruct(out float3 sourcePosition, out float currentDuration)
        {
            sourcePosition = SourcePosition;
            currentDuration = Timer;
        }
    }
}