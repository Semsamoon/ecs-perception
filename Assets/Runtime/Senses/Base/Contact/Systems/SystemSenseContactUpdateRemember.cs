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
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (_, entity) in SystemAPI
                         .Query<TagSenseContactFeel>()
                         .WithDisabled<TagSenseContactFeelRemember>()
                         .WithEntityAccess())
            {
                commands.SetComponentEnabled<TagSenseContactFeelRemember>(entity, true);
            }

            foreach (var (contact, remember) in SystemAPI
                         .Query<RefRO<ComponentSenseContact>, RefRW<ComponentSenseContactRemember>>()
                         .WithAll<TagSenseContactFeel>())
            {
                var (receiver, source) = contact.ValueRO;

                var entitySourceTransform = SystemAPI.GetComponentRO<ComponentSenseSource>(source).ValueRO.Transform;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(entitySourceTransform);
                var receiverRemember = SystemAPI.GetComponentRO<ComponentSenseReceiverRemember>(receiver);

                remember.ValueRW.SourceTransform = sourceTransform.ValueRO;
                remember.ValueRW.Timer = receiverRemember.ValueRO.RememberTime;
            }

            foreach (var (remember, entity) in SystemAPI
                         .Query<RefRW<ComponentSenseContactRemember>>()
                         .WithAll<TagSenseContactFeelRemember>()
                         .WithDisabled<TagSenseContactFeel>()
                         .WithEntityAccess())
            {
                remember.ValueRW.Timer -= deltaTime;

                if (remember.ValueRO.Timer <= 0)
                {
                    commands.SetComponentEnabled<TagSenseContactFeelRemember>(entity, false);
                    commands.SetComponent(entity, new ComponentSenseContactRemember());
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}