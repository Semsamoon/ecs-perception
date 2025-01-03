using Unity.Burst;
using Unity.Entities;
#if UNITY_EDITOR
using ECSPerception.Editor;
using ECSPerception.Editor.Sight;
using Unity.Transforms;
using UnityEngine;
#endif

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true)]
    public partial struct SystemSenseSightRemember : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
#if UNITY_EDITOR
            if (!SystemAPI.HasSingleton<ComponentSenseSightRayDebug>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightRayDebug.Default);
            }
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (_, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>>()
                         .WithAll<BufferSenseSightRemember>()
                         .WithEntityAccess())
            {
                var remembers = SystemAPI.GetBuffer<BufferSenseSightRemember>(receiver);

                for (var i = remembers.Length - 1; i >= 0; i--)
                {
                    var remember = remembers[i];
                    remember.Timer -= deltaTime;

                    if (remember.Timer > 0)
                    {
                        remembers[i] = remember;

#if UNITY_EDITOR
                        var rayDebug = SystemAPI.GetSingleton<ComponentSenseSightRayDebug>();
                        var sourcePositionReal = SystemAPI.GetComponent<LocalToWorld>(remember.Source).Position;
                        ExtendedDebug.DrawOctahedron(remember.SourcePosition, rayDebug.SizeOctahedronSmall, rayDebug.ColorNeutral);
                        ExtendedDebug.DrawOctahedron(sourcePositionReal, rayDebug.SizeOctahedronStandard, rayDebug.ColorNeutral);
                        Debug.DrawLine(remember.SourcePosition, sourcePositionReal, rayDebug.ColorNeutral);
#endif

                        continue;
                    }

                    remembers.RemoveAt(i);
                }
            }
        }
    }
}