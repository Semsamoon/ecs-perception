using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSensePreDestroy))]
    public partial struct SystemSenseSightContactUpdateLinecastDestroy : ISystem
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

            foreach (var linecast in SystemAPI
                         .Query<RefRO<TagSenseSightContactLinecastWait>>()
                         .WithDisabled<TagSenseSightContactConecastResult>())
            {
                var eventDestroy = commands.CreateEntity();
                commands.AddComponent(eventDestroy, new EventSenseLinecastDestroy
                {
                    Entity = linecast.ValueRO.Entity,
                });
                commands.AddComponent(eventDestroy, new EventSenseSightLinecastDestroy());
            }

            commands.Playback(state.EntityManager);
        }
    }
}