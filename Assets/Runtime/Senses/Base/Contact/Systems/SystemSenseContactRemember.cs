using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdate))]
    public partial struct SystemSenseContactRemember : ISystem
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
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (_, entity) in
                     SystemAPI.Query<TagSenseContactRemember>()
                         .WithAll<TagSenseContactFeel>()
                         .WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseContactRemember>(entity, false);
            }

            foreach (var (contact, remember) in
                     SystemAPI.Query<ComponentSenseContact, RefRW<ComponentSenseContactRemember>>()
                         .WithAll<TagSenseContactFeel>())
            {
                var entitySource = SystemAPI.GetComponentRO<ComponentSenseBase>(contact.Source).ValueRO.Entity;
                remember.ValueRW.SourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO;
            }

            foreach (var (contact, remember, entity) in
                     SystemAPI.Query<ComponentSenseContact, RefRW<ComponentSenseContactRemember>>()
                         .WithAll<EventSenseContactFeel>()
                         .WithDisabled<TagSenseContactFeel>()
                         .WithEntityAccess())
            {
                remember.ValueRW.Timer = SystemAPI.GetComponentRO<ComponentSenseReceiverRemember>(contact.Receiver).ValueRO.RememberTime;
                buffer.SetComponentEnabled<TagSenseContactRemember>(entity, true);
            }

            foreach (var (remember, entity) in
                     SystemAPI.Query<RefRW<ComponentSenseContactRemember>>()
                         .WithAll<TagSenseContactRemember>()
                         .WithEntityAccess())
            {
                remember.ValueRW.Timer -= deltaTime;

                if (remember.ValueRO.Timer > 0)
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseContactRemember>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}