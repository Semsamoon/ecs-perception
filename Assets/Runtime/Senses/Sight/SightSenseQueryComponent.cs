using Unity.Entities;
using Unity.Mathematics;

namespace PerceptionECS
{
    public struct SightSenseQueryComponent : IComponentData
    {
        public float3 SourcePosition;
        public float RememberTime;

        public void Deconstruct(out float3 sourcePosition, out float rememberTime)
        {
            sourcePosition = SourcePosition;
            rememberTime = RememberTime;
        }
    }
}