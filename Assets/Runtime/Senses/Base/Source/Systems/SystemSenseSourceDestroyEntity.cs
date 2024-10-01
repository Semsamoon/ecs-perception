using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroySource), OrderLast = true)]
    public partial struct SystemSenseSourceDestroyEntity : ISystem
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

            foreach (var eventDestroy in SystemAPI.Query<RefRO<EventSenseSourceDestroy>>())
            {
                buffer.DestroyEntity(eventDestroy.ValueRO.Entity);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}