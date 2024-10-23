using Unity.Entities;

namespace ECSPerception
{
    public struct ComponentSenseSightLinecast : IComponentData
    {
        public Entity Contact;
    }
}