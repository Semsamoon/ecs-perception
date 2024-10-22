using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateContact))]
    public partial struct SystemSenseContactUpdateRemember : ISystem
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

            foreach (var eventUpdate in SystemAPI.Query<RefRO<EventSenseContactUpdateFeel>>())
            {
                var entityContact = eventUpdate.ValueRO.Entity;

                if (!SystemAPI.HasComponent<ComponentSenseContactRemember>(entityContact))
                {
                    continue;
                }

                if (SystemAPI.IsComponentEnabled<TagSenseContactFeel>(entityContact))
                {
                    commands.SetComponentEnabled<TagSenseContactRemember>(entityContact, false);
                    commands.SetComponent(entityContact, new ComponentSenseContactRemember());
                    continue;
                }

                commands.SetComponentEnabled<TagSenseContactRemember>(entityContact, true);

                var contact = SystemAPI.GetComponentRO<ComponentSenseContact>(entityContact);
                var receiver = SystemAPI.GetComponentRO<ComponentSenseReceiverRemember>(contact.ValueRO.Receiver);
                SystemAPI.GetComponentRW<ComponentSenseContactRemember>(entityContact).ValueRW.Timer = receiver.ValueRO.RememberTime;
            }

            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (remember, entity) in
                     SystemAPI.Query<RefRW<ComponentSenseContactRemember>>().WithAll<TagSenseContactRemember>().WithEntityAccess())
            {
                remember.ValueRW.Timer -= deltaTime;

                if (remember.ValueRO.Timer > 0)
                {
                    continue;
                }

                commands.SetComponentEnabled<TagSenseContactRemember>(entity, false);
                commands.SetComponent(entity, new ComponentSenseContactRemember());
            }

            commands.Playback(state.EntityManager);

            foreach (var (contact, remember) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>, RefRW<ComponentSenseContactRemember>>().WithAll<TagSenseContactFeel>())
            {
                var entitySourceTransform = SystemAPI.GetComponentRO<ComponentSenseSource>(contact.ValueRO.Source).ValueRO.Transform;
                remember.ValueRW.SourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(entitySourceTransform).ValueRO;
            }
        }
    }
}