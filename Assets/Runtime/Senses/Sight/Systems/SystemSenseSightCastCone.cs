using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
#if UNITY_EDITOR
using ECSPerception.Editor.Sight;
using ECSPerception.Editor;
using UnityEngine;
#endif

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true), UpdateAfter(typeof(SystemSenseSightInitialize))]
    public partial struct SystemSenseSightCastCone : ISystem
    {
        private EntityQuery _queryCastsNeed;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _queryCastsNeed = SystemAPI.QueryBuilder()
                .WithAny<BufferSenseSightCastNeed>()
                .Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!_queryCastsNeed.IsEmpty)
            {
                return;
            }

            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<BufferSenseSightActive>()
                         .WithPresent<BufferSenseSightCastNeed>()
                         .WithEntityAccess())
            {
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);

                foreach (var (sourceData, sourceTransform, source) in SystemAPI
                             .Query<RefRO<ComponentSenseSightSource>, RefRO<LocalToWorld>>()
                             .WithEntityAccess())
                {
                    if (!ConecastSenseSight.IsInside(receiverData, receiverTransform, sourceData, sourceTransform, actives.Has(source)))
                    {
                        if (actives.Take(source, out var active) && receiverData.ValueRO.RememberTime > 0)
                        {
                            commands.AppendToBuffer(receiver, new BufferSenseSightRemember
                            {
                                Source = source,
                                SourcePosition = active.SourcePosition,
                                Timer = receiverData.ValueRO.RememberTime,
                            });

#if UNITY_EDITOR
                            if (!SystemAPI.HasComponent<TagSenseDebug>(receiver)
                                || !SystemAPI.IsComponentEnabled<TagSenseDebug>(receiver))
                            {
                                continue;
                            }

                            var rayDebug = SystemAPI.GetSingleton<ComponentSenseSightRayDebug>();
                            var sourcePositionReal = SystemAPI.GetComponent<LocalToWorld>(source).Position;
                            ExtendedDebug.DrawOctahedron(active.SourcePosition, rayDebug.SizeOctahedronSmall, rayDebug.ColorNeutral);
                            ExtendedDebug.DrawOctahedron(sourcePositionReal, rayDebug.SizeOctahedronStandard, rayDebug.ColorNeutral);
                            Debug.DrawLine(active.SourcePosition, sourcePositionReal, rayDebug.ColorNeutral);
#endif
                        }

                        continue;
                    }

                    commands.AppendToBuffer(receiver, new BufferSenseSightCastNeed
                    {
                        Source = source,
                        SourcePosition = sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset),
                    });
                }

                commands.SetComponentEnabled<BufferSenseSightCastNeed>(receiver, true);
            }

            commands.Playback(state.EntityManager);
        }
    }
}