using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateLinecast), OrderLast = true), UpdateAfter(typeof(SystemSenseSightLinecastResult))]
    public partial struct SystemSenseSightLinecastFinish : ISystem
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

            foreach (var (_, entity) in SystemAPI.Query<RefRO<ComponentSenseSightLinecast>>().WithDisabled<TagSenseLinecastWait>().WithEntityAccess())
            {
                var eventDestroy = commands.CreateEntity();
                commands.AddComponent(eventDestroy, new EventSenseLinecastDestroy
                {
                    Entity = entity,
                });
                commands.AddComponent(eventDestroy, new EventSenseSightLinecastDestroy());
            }

            commands.Playback(state.EntityManager);
        }
    }
}