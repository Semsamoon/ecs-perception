using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateSource), OrderLast = true)]
    public partial struct SystemSenseSourceCreateFinish : ISystem
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

            foreach (var (_, entity) in SystemAPI.Query<RefRO<EventSenseSourceCreate>>().WithEntityAccess())
            {
                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}