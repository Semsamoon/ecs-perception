using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreate))]
    public partial struct SystemSenseContactCreate : ISystem
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

            foreach (var (contact, entity) in
                     SystemAPI.Query<ComponentSenseContact>()
                         .WithAll<EventSenseCreate>()
                         .WithEntityAccess())
            {
                if (SystemAPI.HasComponent<ComponentSenseReceiverRemember>(contact.Receiver))
                {
                    buffer.AddComponent<ComponentSenseContactRemember>(entity);
                    buffer.AddComponent<TagSenseContactRemember>(entity);
                    buffer.SetComponentEnabled<TagSenseContactRemember>(entity, false);
                }

                buffer.SetComponentEnabled<EventSenseCreate>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}