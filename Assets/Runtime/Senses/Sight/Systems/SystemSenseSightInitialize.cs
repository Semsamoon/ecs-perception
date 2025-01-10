using Unity.Burst;
using Unity.Entities;
#if UNITY_EDITOR
using ECSPerception.Editor.Sight;
#endif

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true)]
    public partial struct SystemSenseSightInitialize : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<ComponentSenseSightSettings>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightSettings.Default);
            }

            if (!SystemAPI.HasSingleton<ComponentSenseSightState>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightState.Default);
            }

#if UNITY_EDITOR
            if (!SystemAPI.HasSingleton<ComponentSenseSightRayDebug>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightRayDebug.Default);
            }
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SystemAPI.GetSingletonRW<ComponentSenseSightState>().ValueRW.RaycastsAmount = 0;
        }
    }
}