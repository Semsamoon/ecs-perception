using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyLinecast))]
    public partial struct SystemSenseSightLinecastDestroyEntity : ISystem
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

            foreach (var eventDestroy in SystemAPI
                         .Query<RefRO<EventSenseLinecastDestroy>>()
                         .WithAll<EventSenseSightLinecastDestroy>())
            {
                var contact = SystemAPI.GetComponentRO<ComponentSenseSightLinecast>(eventDestroy.ValueRO.Entity).ValueRO.Contact;

                if (state.EntityManager.Exists(contact))
                {
                    commands.SetComponentEnabled<TagSenseSightContactLinecastWait>(contact, false);
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}