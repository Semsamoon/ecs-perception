using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSensePreCreate))]
    public partial struct SystemSenseSightContactUpdateLinecastCreate : ISystem
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

            foreach (var (_, entity) in SystemAPI
                         .Query<TagSenseSightContactConecastResult>()
                         .WithDisabled<TagSenseSightContactLinecastWait>()
                         .WithEntityAccess())
            {
                var eventCreate = commands.CreateEntity();
                commands.AddComponent(eventCreate, new EventSenseLinecastCreate());
                commands.AddComponent(eventCreate, new EventSenseSightLinecastCreate
                {
                    Contact = entity,
                });
            }

            commands.Playback(state.EntityManager);
        }
    }
}