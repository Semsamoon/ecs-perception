using ECSPerception.Sight;
using Unity.Burst;
using Unity.Entities;

namespace ECSPerception.Editor.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true)]
    public partial struct SystemSenseSightDebugInitialize : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<ComponentSenseSightDebug>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightDebug.Default);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }
    }
}