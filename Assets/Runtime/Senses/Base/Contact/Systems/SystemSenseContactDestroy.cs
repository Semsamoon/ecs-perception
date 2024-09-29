using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroy), OrderLast = true)]
    public partial struct SystemSenseContactDestroy : ISystem
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

            foreach (var (_, entityContact) in
                     SystemAPI.Query<ComponentSenseContact>()
                         .WithAll<EventSenseDestroy>()
                         .WithEntityAccess())
            {
                buffer.DestroyEntity(entityContact);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}