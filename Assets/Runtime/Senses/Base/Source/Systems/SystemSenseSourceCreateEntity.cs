using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateSource), OrderFirst = true)]
    public partial struct SystemSenseSourceCreateEntity : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRW<EventSenseSourceCreate>>())
            {
                var entity = state.EntityManager.CreateEntity();
                eventCreate.ValueRW.Entity = entity;
            }

            buffer.Playback(state.EntityManager);
        }
    }
}