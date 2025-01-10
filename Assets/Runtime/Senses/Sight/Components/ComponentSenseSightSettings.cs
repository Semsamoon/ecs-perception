using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightSettings : IComponentData
    {
        public int RaycastsLimit;
        public int RaycastsPerJobAmount;

        public static ComponentSenseSightSettings Default =>
            new()
            {
                RaycastsLimit = 512,
                RaycastsPerJobAmount = 32,
            };

        public void Deconstruct(out int raycastsLimit, out int raycastsPerJobAmount)
        {
            raycastsLimit = RaycastsLimit;
            raycastsPerJobAmount = RaycastsPerJobAmount;
        }
    }
}