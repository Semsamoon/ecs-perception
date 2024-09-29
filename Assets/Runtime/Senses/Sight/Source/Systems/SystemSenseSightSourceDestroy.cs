using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroy))]
    public partial struct SystemSenseSightSourceDestroy : ISystem
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

            foreach (var (_, entitySource) in
                     SystemAPI.Query<ComponentSenseSightSource>()
                         .WithAll<EventSenseDestroy>()
                         .WithEntityAccess())
            {
                foreach (var (contact, entityContact) in
                         SystemAPI.Query<ComponentSenseContact>()
                             .WithAll<TagSenseSight>()
                             .WithEntityAccess())
                {
                    if (contact.Source != entitySource)
                    {
                        continue;
                    }

                    buffer.SetComponentEnabled<EventSenseDestroy>(entityContact, true);
                }

                buffer.DestroyEntity(entitySource);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}