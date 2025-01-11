using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightLimits : IComponentData
    {
        public int RaycastsAmount;
        public int RaycastsPerJobAmount;

        public static ComponentSenseSightLimits Default =>
            new()
            {
                RaycastsAmount = 512,
                RaycastsPerJobAmount = 32,
            };

        public void Deconstruct(out int raycastsLimit, out int raycastsPerJobLimit)
        {
            raycastsLimit = RaycastsAmount;
            raycastsPerJobLimit = RaycastsPerJobAmount;
        }
    }
}