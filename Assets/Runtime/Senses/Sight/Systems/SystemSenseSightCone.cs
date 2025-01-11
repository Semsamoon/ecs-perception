using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true), UpdateAfter(typeof(SystemSenseSightInitialize))]
    public partial struct SystemSenseSightCone : ISystem
    {
        private EntityQuery _queryPossibles;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _queryPossibles = SystemAPI.QueryBuilder()
                .WithAll<BufferSenseSightPossible>()
                .Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!_queryPossibles.IsEmpty)
            {
                return;
            }

            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<BufferSenseSightActive>()
                         .WithPresent<BufferSenseSightPossible>()
                         .WithEntityAccess())
            {
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);

                foreach (var (sourceData, sourceTransform, source) in SystemAPI
                             .Query<RefRO<ComponentSenseSightSource>, RefRO<LocalToWorld>>()
                             .WithEntityAccess())
                {
                    if (!UtilSenseSightCone.Cast(receiverData, receiverTransform, sourceData, sourceTransform, actives.Has(source)))
                    {
                        if (actives.Take(source, out var active) && receiverData.ValueRO.RememberTime > 0)
                        {
                            commands.AppendToBuffer(receiver, new BufferSenseSightMemory
                            {
                                Source = source,
                                SourcePosition = active.SourcePosition,
                                Timer = receiverData.ValueRO.RememberTime,
                            });
                        }

                        continue;
                    }

                    commands.AppendToBuffer(receiver, new BufferSenseSightPossible
                    {
                        Source = source,
                        SourcePosition = sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset),
                    });
                }

                commands.SetComponentEnabled<BufferSenseSightPossible>(receiver, true);
            }

            commands.Playback(state.EntityManager);
        }
    }
}