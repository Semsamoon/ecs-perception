using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyReceiver), OrderLast = true)]
    public partial struct SystemSenseReceiverDestroyEntity : ISystem
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

            foreach (var eventDestroy in SystemAPI.Query<RefRO<EventSenseReceiverDestroy>>())
            {
                buffer.DestroyEntity(eventDestroy.ValueRO.Entity);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}