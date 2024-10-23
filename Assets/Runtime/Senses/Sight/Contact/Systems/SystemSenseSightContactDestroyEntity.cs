using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyContact))]
    public partial struct SystemSenseSightContactDestroyEntity : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var eventDestroy in SystemAPI
                         .Query<RefRO<EventSenseContactDestroy>>()
                         .WithAll<EventSenseSightContactDestroy>())
            {
                var contact = eventDestroy.ValueRO.Entity;

                if (SystemAPI.IsComponentEnabled<TagSenseSightContactLinecastWait>(contact))
                {
                    var linecast = SystemAPI.GetComponentRO<TagSenseSightContactLinecastWait>(contact).ValueRO.Entity;

                    var eventLinecastDestroy = commands.CreateEntity();
                    commands.AddComponent(eventLinecastDestroy, new EventSenseLinecastDestroy
                    {
                        Entity = linecast,
                    });
                    commands.AddComponent(eventLinecastDestroy, new EventSenseSightLinecastDestroy());
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}