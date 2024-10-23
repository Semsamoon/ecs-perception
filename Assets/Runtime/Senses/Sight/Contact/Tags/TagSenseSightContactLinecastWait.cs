using Unity.Entities;

namespace ECSPerception
{
    public struct TagSenseSightContactLinecastWait : IComponentData, IEnableableComponent
    {
        public Entity Entity;
    }
}