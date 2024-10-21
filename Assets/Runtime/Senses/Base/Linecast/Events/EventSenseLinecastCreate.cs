using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseLinecastCreate : IComponentData
    {
        public Entity Entity;
        public Entity Contact;
    }
}