using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateContact))]
    public partial struct SystemSenseSightContactCreateComponents : ISystem
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

            foreach (var (eventCreate, _) in SystemAPI.Query<RefRO<EventSenseContactCreate>, RefRO<EventSenseSightContactCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                buffer.AddComponent(entity, new ComponentSenseSightContact());
                buffer.AddComponent(entity, new TagSenseSightContactConeIn());
                buffer.SetComponentEnabled<TagSenseSightContactConeIn>(entity, false);
                buffer.AddComponent(entity, new TagSenseContactLineCast());
                buffer.SetComponentEnabled<TagSenseContactLineCast>(entity, false);
                buffer.AddComponent(entity, new TagSenseContactLineUp());
                buffer.SetComponentEnabled<TagSenseContactLineUp>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}