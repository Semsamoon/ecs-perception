using ECSPerception.Groups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses), OrderLast = true)]
    public partial struct SystemSenseSightCastExecute : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<ComponentSenseSightSettings>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightSettings.Default);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var (raycastsAmount, raycastsPerJobAmount) = SystemAPI.GetSingleton<ComponentSenseSightSettings>();
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var commands = new EntityCommandBuffer(Allocator.Temp);

            var raycasts = new NativeArray<RaycastSenseSightCast>(raycastsAmount, Allocator.TempJob);
            var raycastsMeta = new NativeArray<RaycastSenseSightCastMeta>(raycastsAmount, Allocator.TempJob);
            var results = new NativeArray<bool>(raycastsAmount, Allocator.TempJob);
            var index = 0;

            foreach (var (receiverData, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>>()
                         .WithAll<BufferSenseSightCastPending>()
                         .WithEntityAccess())
            {
                var pending = SystemAPI.GetBuffer<BufferSenseSightCastPending>(receiver);
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);
                var remembers = SystemAPI.GetBuffer<BufferSenseSightRemember>(receiver);
                var rememberTime = receiverData.ValueRO.RememberTime;

                var currentSource = Entity.Null;
                var unsafeIndex = index;

                for (var j = pending.Length - 1; j >= 0 && unsafeIndex < raycastsAmount; j--, unsafeIndex++)
                {
                    raycasts[unsafeIndex] = pending[j].Raycast;
                    raycastsMeta[unsafeIndex] = new RaycastSenseSightCastMeta
                        { Actives = actives, Remembers = remembers, RememberTime = rememberTime };

                    if (currentSource != pending[j].Raycast.Source || j == 0)
                    {
                        currentSource = pending[j].Raycast.Source;
                        pending.RemoveRange(j, unsafeIndex - index + 1);
                        index = unsafeIndex + 1;
                    }
                }

                if (pending.Length == 0)
                {
                    commands.SetComponentEnabled<BufferSenseSightCastPending>(receiver, false);
                }

                if (index == raycastsAmount)
                {
                    break;
                }
            }

            if (index == 0)
            {
                commands.Playback(state.EntityManager);
                raycasts.Dispose();
                raycastsMeta.Dispose();
                results.Dispose();
                return;
            }

            var jobRaycast = new RaycastSenseSightJobCast
            {
                CollisionWorld = collisionWorld,
                Raycasts = raycasts,
                Results = results,
            }.Schedule(index, raycastsPerJobAmount, state.Dependency);

            state.Dependency = jobRaycast;
            jobRaycast.Complete();

            var resultsEnumerator = new RaycastSenseSightResult(raycasts, raycastsMeta, results, index);

            while (resultsEnumerator.MoveNext())
            {
                var (raycast, raycastMeta, result) = resultsEnumerator.GetCurrent();

                if (!result)
                {
                    if (raycastMeta.Actives.Remove(raycast.Source) && raycastMeta.RememberTime > 0)
                    {
                        commands.AppendToBuffer(raycast.Receiver, new BufferSenseSightRemember(raycast.Source, raycastMeta.RememberTime));
                    }

                    continue;
                }

                if (raycastMeta.Actives.Has(raycast.Source))
                {
                    continue;
                }

                commands.AppendToBuffer(raycast.Receiver, new BufferSenseSightActive(raycast.Source));
                raycastMeta.Remembers.Remove(raycast.Source);
            }

            commands.Playback(state.EntityManager);
            raycasts.Dispose();
            raycastsMeta.Dispose();
            results.Dispose();
        }
    }
}