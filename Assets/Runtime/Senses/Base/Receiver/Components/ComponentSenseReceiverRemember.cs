using Unity.Entities;

namespace PerceptionECS
{
    public struct ComponentSenseReceiverRemember : IComponentData
    {
        public float RememberTime;
    }
}