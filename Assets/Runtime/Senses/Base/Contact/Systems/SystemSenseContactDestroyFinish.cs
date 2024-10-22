using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyContact), OrderLast = true), UpdateAfter(typeof(SystemSenseReceiverDestroyEntity))]
    public partial struct SystemSenseContactDestroyFinish : ISystem
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

            foreach (var (_, entity) in SystemAPI.Query<RefRO<EventSenseContactDestroy>>().WithEntityAccess())
            {
                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}