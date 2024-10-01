using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateContact))]
    public partial struct SystemSenseSightContactCreateComponents : ISystem
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

            foreach (var (eventCreate, _) in SystemAPI.Query<RefRO<EventSenseContactCreate>, RefRO<EventSenseSightContactCreate>>())
            {
                buffer.AddComponent(eventCreate.ValueRO.Entity, new ComponentSenseSightContact());
            }

            buffer.Playback(state.EntityManager);
        }
    }
}