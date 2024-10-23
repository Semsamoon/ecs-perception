using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyLinecast), OrderLast = true), UpdateAfter(typeof(SystemSenseLinecastDestroyEntity))]
    public partial struct SystemSenseLinecastDestroyFinish : ISystem
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

            foreach (var (_, entity) in SystemAPI.Query<RefRO<EventSenseLinecastDestroy>>().WithEntityAccess())
            {
                commands.DestroyEntity(entity);
            }

            commands.Playback(state.EntityManager);
        }
    }
}