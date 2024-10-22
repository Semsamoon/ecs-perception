using Unity.Entities;

namespace ECSPerception
{
    public struct ComponentSenseReceiverRemember : IComponentData
    {
        public float RememberTime;
    }
}