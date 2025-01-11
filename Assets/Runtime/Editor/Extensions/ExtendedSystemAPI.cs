using Unity.Entities;

namespace ECSPerception.Editor.Extensions
{
    public static class ExtendedSystemAPI
    {
        public static bool HasComponentEnabled<T>(ref SystemState state, in Entity entity) where T : unmanaged, IComponentData, IEnableableComponent
        {
            return state.EntityManager.HasComponent<T>(entity) && state.EntityManager.IsComponentEnabled<T>(entity);
        }
    }
}