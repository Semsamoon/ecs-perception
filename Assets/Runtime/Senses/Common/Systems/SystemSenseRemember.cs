using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile]
    public partial struct SystemSenseRemember : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (interaction, remember) in
                     SystemAPI.Query<ComponentSenseInteraction, RefRW<ComponentSenseRemember>>().WithAll<TagSenseFeel>())
            {
                var (_, entitySource) = interaction;
                remember.ValueRW.SourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO;
            }

            foreach (var (interaction, remember, entityInteraction) in
                     SystemAPI.Query<ComponentSenseInteraction, RefRW<ComponentSenseRemember>>().WithAll<TagSenseTransitionFeelToNeglect>().WithEntityAccess())
            {
                var (entityReceiver, _) = interaction;
                ref var rememberValue = ref remember.ValueRW;

                rememberValue.Timer = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO.RememberTime;

                buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, true);
            }

            foreach (var (_, entityInteraction) in
                     SystemAPI.Query<TagSenseRemember>().WithAll<TagSenseTransitionNeglectToFeel>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
            }

            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (remember, entityInteraction) in
                     SystemAPI.Query<RefRW<ComponentSenseRemember>>().WithAll<TagSenseRemember>().WithEntityAccess())
            {
                remember.ValueRW.Timer -= deltaTime;

                if (remember.ValueRO.Timer > 0)
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}