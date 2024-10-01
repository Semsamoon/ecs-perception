using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateContact), OrderFirst = true)]
    public partial struct SystemSenseContactCreateEntity : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRW<EventSenseContactCreate>>())
            {
                var entity = state.EntityManager.CreateEntity();
                eventCreate.ValueRW.Entity = entity;
            }

            buffer.Playback(state.EntityManager);
        }
    }
}