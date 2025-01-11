using Unity.Burst;
using Unity.Entities;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true)]
    public partial struct SystemSenseSightInitialize : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<ComponentSenseSightLimits>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightLimits.Default);
            }

            if (!SystemAPI.HasSingleton<ComponentSenseSightCurrent>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightCurrent.Default);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SystemAPI.GetSingletonRW<ComponentSenseSightCurrent>().ValueRW.RaycastsAmount = 0;
        }
    }
}