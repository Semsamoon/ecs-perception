using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseContactCreate : IComponentData
    {
        public Entity Entity;
        public Entity Receiver;
        public Entity Source;
    }
}