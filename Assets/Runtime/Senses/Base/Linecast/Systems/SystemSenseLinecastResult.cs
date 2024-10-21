using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateLinecast), OrderLast = true)]
    public partial struct SystemSenseLinecastResult : ISystem
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

            foreach (var linecast in
                     SystemAPI.Query<RefRO<ComponentSenseLinecast>>().WithAll<TagSenseLinecastSuccess>())
            {
                buffer.SetComponentEnabled<TagSenseContactLineUp>(linecast.ValueRO.Owner, true);
            }

            foreach (var linecast in
                     SystemAPI.Query<RefRO<ComponentSenseLinecast>>().WithDisabled<TagSenseLinecastWait>())
            {
                buffer.SetComponentEnabled<TagSenseContactLineCast>(linecast.ValueRO.Owner, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}