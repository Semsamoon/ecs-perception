using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var eventCreate in SystemAPI
                         .Query<RefRO<EventSenseLinecastCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                commands.AddComponent(entity, new TagSenseLinecastWait());
                commands.AddComponent(entity, new TagSenseLinecastResult());
                commands.SetComponentEnabled<TagSenseLinecastResult>(entity, false);
            }

            commands.Playback(state.EntityManager);
        }
    }
}