using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightCurrent : IComponentData
    {
        public int RaycastsAmount;

        public static ComponentSenseSightCurrent Default => new();
    }
}