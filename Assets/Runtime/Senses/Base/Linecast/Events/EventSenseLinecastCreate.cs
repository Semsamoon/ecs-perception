using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseLinecastCreate : IComponentData
    {
        public Entity Entity;
        public Entity Contact;
    }
}