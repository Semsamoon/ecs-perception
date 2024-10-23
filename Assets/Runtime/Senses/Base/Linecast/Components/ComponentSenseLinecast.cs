using Unity.Entities;
using Unity.Mathematics;

namespace ECSPerception
{
    public struct ComponentSenseLinecast : IComponentData
    {
        public Entity ReceiverTransform;
        public Entity ReceiverOwner;
        public float3 ReceiverOffset; 
        public Entity SourceTransform;
        public Entity SourceOwner;
        public float3 SourceOffset;
    }
}