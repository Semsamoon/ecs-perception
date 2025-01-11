using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
#if UNITY_EDITOR
using ECSPerception.Editor;
using ECSPerception.Editor.Extensions;
using ECSPerception.Editor.Sight;
using Unity.Transforms;
using UnityEngine;
#endif

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderLast = true)]
    public partial struct SystemSenseSightExecute : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var raycastsAmount = SystemAPI.GetSingleton<ComponentSenseSightCurrent>().RaycastsAmount;
            var raycastsPerJobLimit = SystemAPI.GetSingleton<ComponentSenseSightLimits>().RaycastsPerJobAmount;
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var commands = new EntityCommandBuffer(Allocator.Temp);

            var raycasts = new NativeArray<BufferSenseSightExecute>(raycastsAmount, Allocator.TempJob);
            var raycastsMeta = new NativeArray<UtilSenseSightRayMeta>(raycastsAmount, Allocator.TempJob);
            var results = new NativeArray<bool>(raycastsAmount, Allocator.TempJob);
            var index = 0;

            foreach (var (receiverData, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>>()
                         .WithAll<BufferSenseSightExecute>()
                         .WithEntityAccess())
            {
                var executes = SystemAPI.GetBuffer<BufferSenseSightExecute>(receiver);
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);
                var memories = SystemAPI.GetBuffer<BufferSenseSightMemory>(receiver);
                var rememberTime = receiverData.ValueRO.RememberTime;

                for (var j = 0; j < executes.Length; j++, index++)
                {
                    raycasts[index] = executes[j];
                    raycastsMeta[index] = new UtilSenseSightRayMeta
                    {
                        Actives = actives,
                        Memories = memories,
                        RememberTime = rememberTime,
                    };
                }

                executes.Clear();
                commands.SetComponentEnabled<BufferSenseSightExecute>(receiver, false);
            }

            if (index == 0)
            {
                commands.Playback(state.EntityManager);
                raycasts.Dispose();
                raycastsMeta.Dispose();
                results.Dispose();
                return;
            }

            var jobRaycast = new UtilSenseSightRayJob
            {
                CollisionWorld = collisionWorld,
                Executes = raycasts,
                Results = results,
            }.Schedule(index, raycastsPerJobLimit, state.Dependency);

            state.Dependency = jobRaycast;
            jobRaycast.Complete();

#if UNITY_EDITOR
            var debug = SystemAPI.GetSingleton<ComponentSenseSightDebug>();

            for (var i = 0; i < index; i++)
            {
                if (ExtendedSystemAPI.HasComponentEnabled<TagSenseDebug>(ref state, raycasts[i].Receiver))
                {
                    var receiverPosition = raycasts[i].ReceiverPosition;
                    var sourcePosition = raycasts[i].SourcePosition;
                    var sourceTransform = SystemAPI.GetComponent<LocalToWorld>(raycasts[i].Source);
                    var offset = SystemAPI.GetComponent<ComponentSenseSightSource>(raycasts[i].Source).Offset;
                    var sourceCurrentPosition = sourceTransform.Value.TransformPoint(offset);
                    var color = results[i] ? debug.RaySuccessColor : debug.RayFailureColor;

                    Debug.DrawLine(receiverPosition, sourcePosition, color);
                    Debug.DrawLine(sourcePosition, sourceCurrentPosition, color);
                    ExtendedDebug.DrawOctahedron(sourcePosition, debug.SizeOctahedronSmall, color);
                }
            }
#endif

            var resultsEnumerator = new UtilSenseSightRayEnumerator(raycasts, raycastsMeta, results);

            while (resultsEnumerator.MoveNext())
            {
                var (executed, raycastMeta, result) = resultsEnumerator.GetCurrent();

                if (!result)
                {
                    if (raycastMeta.Actives.Take(executed.Source, out var active) && raycastMeta.RememberTime > 0)
                    {
                        commands.AppendToBuffer(executed.Receiver, new BufferSenseSightMemory
                        {
                            Source = executed.Source,
                            SourcePosition = active.SourcePosition,
                            Timer = raycastMeta.RememberTime,
                        });
                    }

                    continue;
                }

                if (!raycastMeta.Actives.Has(executed.Source))
                {
                    commands.AppendToBuffer(executed.Receiver, new BufferSenseSightActive
                    {
                        Source = executed.Source,
                        SourcePosition = executed.SourcePosition,
                    });
                    raycastMeta.Memories.Remove(executed.Source);
                }
            }

            commands.Playback(state.EntityManager);
            raycasts.Dispose();
            raycastsMeta.Dispose();
            results.Dispose();
        }
    }
}