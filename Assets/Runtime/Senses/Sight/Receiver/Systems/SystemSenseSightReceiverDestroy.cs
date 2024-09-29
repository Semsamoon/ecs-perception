using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroy))]
    public partial struct SystemSenseSightReceiverDestroy : ISystem
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

            foreach (var (_, entityReceiver) in
                     SystemAPI.Query<ComponentSenseSightReceiver>()
                         .WithAll<EventSenseDestroy>()
                         .WithEntityAccess())
            {
                foreach (var (contact, entityContact) in
                         SystemAPI.Query<ComponentSenseContact>()
                             .WithAll<TagSenseSight>()
                             .WithEntityAccess())
                {
                    if (contact.Receiver != entityReceiver)
                    {
                        continue;
                    }

                    buffer.SetComponentEnabled<EventSenseDestroy>(entityContact, true);
                }

                buffer.DestroyEntity(entityReceiver);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}