using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateLinecast), OrderFirst = true), UpdateAfter(typeof(SystemSenseLinecastCreateEntity))]
    public partial struct SystemSenseLinecastCreateComponents : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseLinecastCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                buffer.AddComponent(entity, new TagSenseLinecastWait());
                buffer.AddComponent(entity, new TagSenseLinecastSuccess());
                buffer.SetComponentEnabled<TagSenseLinecastSuccess>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}