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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (eventCreate, _) in SystemAPI.Query<RefRO<EventSenseContactCreate>, RefRO<EventSenseSightContactCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                commands.AddComponent(entity, new ComponentSenseSightContact());
                commands.AddComponent(entity, new TagSenseSightContactConecastResult());
                commands.SetComponentEnabled<TagSenseSightContactConecastResult>(entity, false);
                commands.AddComponent(entity, new TagSenseSightContactLinecastWait());
                commands.SetComponentEnabled<TagSenseSightContactLinecastWait>(entity, false);
                commands.AddComponent(entity, new TagSenseSightContactLinecastResult());
                commands.SetComponentEnabled<TagSenseSightContactLinecastResult>(entity, false);
            }

            commands.Playback(state.EntityManager);
        }
    }
}