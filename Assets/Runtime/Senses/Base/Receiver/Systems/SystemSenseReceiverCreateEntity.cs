using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateReceiver), OrderFirst = true)]
    public partial struct SystemSenseReceiverCreateEntity : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRW<EventSenseReceiverCreate>>())
            {
                var entity = state.EntityManager.CreateEntity();
                eventCreate.ValueRW.Entity = entity;
            }

            buffer.Playback(state.EntityManager);
        }
    }
}