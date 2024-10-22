using Unity.Entities;

namespace ECSPerception
{
    public struct ComponentSenseContact : IComponentData
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