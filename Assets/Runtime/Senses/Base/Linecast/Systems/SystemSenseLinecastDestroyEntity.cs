using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyLinecast), OrderLast = true)]
    public partial struct SystemSenseLinecastDestroyEntity : ISystem
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

            foreach (var eventDestroy in SystemAPI
                         .Query<RefRO<EventSenseLinecastDestroy>>())
            {
                commands.DestroyEntity(eventDestroy.ValueRO.Entity);
            }

            commands.Playback(state.EntityManager);
        }
    }
}