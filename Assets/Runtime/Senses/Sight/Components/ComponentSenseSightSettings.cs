using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightSettings : IComponentData
    {
        public int RaycastsLimitAmount;
        public int RaycastsPerJobAmount;

        public static ComponentSenseSightSettings Default =>
            new()
            {
                RaycastsLimitAmount = 512,
                RaycastsPerJobAmount = 32,
            };

        public void Deconstruct(out int raycastsLimitAmount, out int raycastsPerJobAmount)
        {
            raycastsLimitAmount = RaycastsLimitAmount;
            raycastsPerJobAmount = RaycastsPerJobAmount;
        }
    }
}