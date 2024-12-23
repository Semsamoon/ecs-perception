using ECSPerception.Groups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses))]
    public partial struct SystemSenseSightSingleCast : ISystem
    {
        private int _raycastsLimitAmount;
        private int _raycastsPerJobAmount;
        private int _raycastCurrentIndex;

        private EntityCommandBuffer _commands;
        private NativeArray<RaycastSenseSightData> _raycasts;

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
            (_raycastsLimitAmount, _raycastsPerJobAmount) = SystemAPI.GetSingleton<ComponentSenseSightSettings>();
            _raycastCurrentIndex = 0;

            _commands = new EntityCommandBuffer(Allocator.Temp);
            _raycasts = new NativeArray<RaycastSenseSightData>(_raycastsLimitAmount, Allocator.TempJob);

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
                        continue;
                    }

                    remembers.RemoveAt(i);
                }
            }

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<BufferSenseSightNeedCast, BufferSenseSightActive, BufferSenseSightRemember>()
                         .WithEntityAccess())
            {
                var needCasts = SystemAPI.GetBuffer<BufferSenseSightNeedCast>(receiver);
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);

                for (var i = needCasts.Length - 1; i >= 0; i--)
                {
                    if (_raycastCurrentIndex == _raycastsLimitAmount)
                    {
                        break;
                    }

                    var source = needCasts[i].Source;
                    var sourceData = SystemAPI.GetComponentRO<ComponentSenseSightSource>(source);
                    var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(source);

                    if (!ConecastSenseSight.IsInside(receiverData, receiverTransform, sourceData, sourceTransform, actives.Has(source)))
                    {
                        needCasts.RemoveAt(i);
                        if (actives.Remove(source) && receiverData.ValueRO.RememberTime > 0)
                        {
                            _commands.AppendToBuffer(receiver, new BufferSenseSightRemember(source, receiverData.ValueRO.RememberTime));
                        }

                        continue;
                    }

                    needCasts.RemoveAt(i);
                    _raycasts[_raycastCurrentIndex] = new RaycastSenseSightData(
                        receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset), receiver,
                        sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset), source);
                    _raycastCurrentIndex++;
                }

                if (_raycastCurrentIndex == _raycastsLimitAmount)
                {
                    break;
                }
            }

            if (_raycastCurrentIndex == _raycastsLimitAmount)
            {
                var results = ExecuteRaycasts(ref state);
                HandleRaycastsResults(ref state, results);

                _commands.Playback(state.EntityManager);
                _raycasts.Dispose();
                results.Dispose();

                return;
            }

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<BufferSenseSightActive, BufferSenseSightNeedCast, BufferSenseSightRemember>()
                         .WithEntityAccess())
            {
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);

                foreach (var (sourceData, sourceTransform, source) in SystemAPI
                             .Query<RefRO<ComponentSenseSightSource>, RefRO<LocalToWorld>>()
                             .WithEntityAccess())
                {
                    if (!ConecastSenseSight.IsInside(receiverData, receiverTransform, sourceData, sourceTransform, actives.Has(source)))
                    {
                        if (actives.Remove(source) && receiverData.ValueRO.RememberTime > 0)
                        {
                            _commands.AppendToBuffer(receiver, new BufferSenseSightRemember(source, receiverData.ValueRO.RememberTime));
                        }

                        continue;
                    }

                    if (_raycastCurrentIndex == _raycastsLimitAmount)
                    {
                        _commands.AppendToBuffer(receiver, new BufferSenseSightNeedCast(source));
                        continue;
                    }

                    _raycasts[_raycastCurrentIndex] = new RaycastSenseSightData(
                        receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset), receiver,
                        sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset), source);
                    _raycastCurrentIndex++;
                }
            }

            {
                var results = ExecuteRaycasts(ref state);
                HandleRaycastsResults(ref state, results);

                _commands.Playback(state.EntityManager);
                _raycasts.Dispose();
                results.Dispose();
            }
        }

        [BurstCompile]
        private NativeArray<bool> ExecuteRaycasts(ref SystemState state)
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var results = new NativeArray<bool>(_raycastCurrentIndex, Allocator.TempJob);

            var handle = new RaycastSenseSightJob
            {
                CollisionWorld = collisionWorld,
                Raycasts = _raycasts,
                Results = results,
            }.Schedule(_raycastCurrentIndex, _raycastsPerJobAmount, state.Dependency);
            handle.Complete();

            return results;
        }

        [BurstCompile]
        private void HandleRaycastsResults(ref SystemState state, NativeArray<bool> results)
        {
            for (var i = 0; i < _raycastCurrentIndex; i++)
            {
                var receiver = _raycasts[i].Receiver;
                var source = _raycasts[i].Source;
                var receiverData = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(receiver);
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);
                var remembers = SystemAPI.GetBuffer<BufferSenseSightRemember>(receiver);

                if (!results[i])
                {
                    if (actives.Remove(source) && receiverData.ValueRO.RememberTime > 0)
                    {
                        _commands.AppendToBuffer(receiver, new BufferSenseSightRemember(source, receiverData.ValueRO.RememberTime));
                    }

                    continue;
                }

                if (!actives.Has(source))
                {
                    _commands.AppendToBuffer(receiver, new BufferSenseSightActive(source));
                    remembers.Remove(source);
                }
            }
        }
    }
}