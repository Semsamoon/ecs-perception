using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var linecast in
                     SystemAPI.Query<RefRO<ComponentSenseLinecast>>().WithAll<TagSenseLinecastSuccess>())
            {
                commands.SetComponentEnabled<TagSenseContactLineUp>(linecast.ValueRO.Owner, true);
            }

            foreach (var linecast in
                     SystemAPI.Query<RefRO<ComponentSenseLinecast>>().WithDisabled<TagSenseLinecastWait>())
            {
                commands.SetComponentEnabled<TagSenseContactLineCast>(linecast.ValueRO.Owner, false);
            }

            commands.Playback(state.EntityManager);
        }
    }
}