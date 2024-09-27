using Unity.Entities;

namespace PerceptionECS
{
    public struct ComponentSenseInteraction : IComponentData
    {
        public Entity Receiver;
        public Entity Source;

        public void Deconstruct(out Entity receiver, out Entity source)
        {
            receiver = Receiver;
            source = Source;
        }
    }
}