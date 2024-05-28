using Unity.Entities;
using Unity.Mathematics;

namespace PerceptionECS
{
    public struct SightSenseQueryComponent : IComponentData
    {
        public Entity Listener;
        public Entity Source;
        public float3 SourcePosition;
        public float RememberTime;

        public void Deconstruct(out Entity listener, out Entity source)
        {
            listener = Listener;
            source = Source;
        }

        public void Deconstruct(out Entity listener, out Entity source, out float3 sourcePosition)
        {
            listener = Listener;
            source = Source;
            sourcePosition = SourcePosition;
        }

        public void Deconstruct(out Entity listener, out Entity source, out float3 sourcePosition, out float rememberTime)
        {
            listener = Listener;
            source = Source;
            sourcePosition = SourcePosition;
            rememberTime = RememberTime;
        }
    }
}