using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightState : IComponentData
    {
        public int RaycastsAmount;

        public static ComponentSenseSightState Default => new();
    }
}