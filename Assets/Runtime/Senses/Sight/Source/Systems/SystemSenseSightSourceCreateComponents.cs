using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateSource))]
    public partial struct SystemSenseSightSourceCreateComponents : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var eventCreate in SystemAPI
                         .Query<RefRO<EventSenseSourceCreate>>()
                         .WithAll<EventSenseSightSourceCreate>())
            {
                commands.AddComponent(eventCreate.ValueRO.Entity, new ComponentSenseSightSource());
            }

            commands.Playback(state.EntityManager);
        }
    }
}