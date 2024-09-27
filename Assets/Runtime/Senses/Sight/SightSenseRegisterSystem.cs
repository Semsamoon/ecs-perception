using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace PerceptionECS
{
    [BurstCompile]
    [UpdateBefore(typeof(SightSenseVisibilitySystem))]
    public partial struct SightSenseRegisterSystem : ISystem
    {
        private EntityArchetype _interactionArchetype;

        public void OnCreate(ref SystemState state)
        {
            var queryTypes = new NativeArray<ComponentType>(4, Allocator.Temp);
            queryTypes[0] = typeof(ComponentSenseInteraction);
            queryTypes[1] = typeof(ComponentSenseInteractionRemember);
            queryTypes[2] = typeof(TagSenseFeel);
            queryTypes[3] = typeof(SightSenseRememberTag);
            _interactionArchetype = state.EntityManager.CreateArchetype(queryTypes);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityReceiver) in
                     SystemAPI
                         .Query<SightSenseListenerComponent>()
                         .WithAll<SightSenseListenerRegisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (_, entitySource) in
                         SystemAPI
                             .Query<SightSenseSourceTag>()
                             .WithDisabled<SightSenseSourceRegisterTag>()
                             .WithEntityAccess())
                {
                    if (entitySource == entityReceiver) continue;

                    var entityInteraction = buffer.CreateEntity(_interactionArchetype);
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteraction
                    {
                        Receiver = entityReceiver, Source = entitySource
                    });
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteractionRemember
                    {
                        SourcePosition = float3.zero, Duration = 0
                    });
                    buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<SightSenseRememberTag>(entityInteraction, false);
                }

                buffer.SetComponentEnabled<SightSenseListenerRegisterTag>(entityReceiver, false);
            }

            buffer.Playback(state.EntityManager);

            buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entitySource) in
                     SystemAPI
                         .Query<SightSenseSourceTag>()
                         .WithAll<SightSenseSourceRegisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (_, entityReceiver) in
                         SystemAPI
                             .Query<SightSenseListenerComponent>()
                             .WithDisabled<SightSenseListenerRegisterTag>()
                             .WithEntityAccess())
                {
                    if (entityReceiver == entitySource) continue;

                    var entityInteraction = buffer.CreateEntity(_interactionArchetype);
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteraction
                    {
                        Receiver = entityReceiver, Source = entitySource
                    });
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteractionRemember
                    {
                        SourcePosition = float3.zero, Duration = 0,
                    });
                    buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<SightSenseRememberTag>(entityInteraction, false);
                }

                buffer.SetComponentEnabled<SightSenseSourceRegisterTag>(entitySource, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}