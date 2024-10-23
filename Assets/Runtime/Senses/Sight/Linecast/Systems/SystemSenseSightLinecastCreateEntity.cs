using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateLinecast))]
    public partial struct SystemSenseSightLinecastCreateEntity : ISystem
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

            foreach (var (eventCreate, eventSightCreate) in
                     SystemAPI.Query<RefRO<EventSenseLinecastCreate>, RefRO<EventSenseSightLinecastCreate>>())
            {
                var linecast = SystemAPI.GetComponentRW<TagSenseSightContactLinecastWait>(eventSightCreate.ValueRO.Contact);
                linecast.ValueRW.Entity = eventCreate.ValueRO.Entity;

                commands.SetComponentEnabled<TagSenseSightContactLinecastWait>(eventSightCreate.ValueRO.Contact, true);
            }

            commands.Playback(state.EntityManager);
        }
    }
}