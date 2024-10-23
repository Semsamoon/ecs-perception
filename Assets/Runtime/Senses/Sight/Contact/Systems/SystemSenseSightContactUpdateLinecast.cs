using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateContact), OrderFirst = true), UpdateAfter(typeof(SystemSenseSightContactUpdateConecast))]
    public partial struct SystemSenseSightContactUpdateLinecast : ISystem
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

            foreach (var (_, entity) in
                     SystemAPI.Query<TagSenseSightContactConecastResult>().WithDisabled<TagSenseSightContactLinecastWait>().WithEntityAccess())
            {
                var eventCreate = commands.CreateEntity();
                commands.AddComponent(eventCreate, new EventSenseLinecastCreate());
                commands.AddComponent(eventCreate, new EventSenseSightLinecastCreate
                {
                    Contact = entity,
                });
            }

            foreach (var linecast in
                     SystemAPI.Query<RefRO<TagSenseSightContactLinecastWait>>().WithDisabled<TagSenseSightContactConecastResult>())
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