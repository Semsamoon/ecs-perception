using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
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
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (eventCreate, _) in SystemAPI.Query<RefRO<EventSenseSourceCreate>, RefRO<EventSenseSightSourceCreate>>())
            {
                buffer.AddComponent(eventCreate.ValueRO.Entity, new ComponentSenseSightSource());
            }

            buffer.Playback(state.EntityManager);
        }
    }
}