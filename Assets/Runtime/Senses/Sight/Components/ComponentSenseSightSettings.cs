using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct ComponentSenseSightSettings : IComponentData
    {
        public int RaycastsAmount;
        public int RaycastsPerJobAmount;

        public static ComponentSenseSightSettings Default =>
            new()
            {
                RaycastsAmount = 512,
                RaycastsPerJobAmount = 32,
            };

        public void Deconstruct(out int raycastsAmount, out int raycastsPerJobAmount)
        {
            raycastsAmount = RaycastsAmount;
            raycastsPerJobAmount = RaycastsPerJobAmount;
        }
    }
}