using ECSPerception.Groups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses))]
    public partial struct SystemSenseSightMultiCastExecute : ISystem
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

            foreach (var (_, receiver) in SystemAPI
                         .Query<TagSenseSightMultiCast>()
                         .WithAll<BufferSenseSightActive, BufferSenseSightCast, BufferSenseSightRemember>()
                         .WithEntityAccess())
            {
                var casts = SystemAPI.GetBuffer<BufferSenseSightCast>(receiver);

                for (var i = casts.Length - 1; i >= 0; i--)
                {
                    if (_raycastCurrentIndex == _raycastsLimitAmount)
                    {
                        break;
                    }

                    _raycasts[_raycastCurrentIndex] = casts[i].Raycast;
                    casts.RemoveAt(i);
                    _raycastCurrentIndex++;
                }

                if (_raycastCurrentIndex == _raycastsLimitAmount)
                {
                    break;
                }
            }

            var results = ExecuteRaycasts(ref state);
            HandleRaycastsResults(ref state, results);

            _commands.Playback(state.EntityManager);
            _raycasts.Dispose();
            results.Dispose();
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

                var succeed = false;

                while (i < _raycastCurrentIndex && _raycasts[i].Source == source)
                {
                    if (results[i])
                    {
                        succeed = true;
                    }

                    i++;
                }

                if (!succeed)
                {
                    if (actives.Remove(source) && receiverData.ValueRO.RememberTime > 0)
                    {
                        _commands.AppendToBuffer(receiver, new BufferSenseSightRemember(source, receiverData.ValueRO.RememberTime));
                    }

                    continue;
                }

                if (actives.Has(source))
                {
                    continue;
                }

                _commands.AppendToBuffer(receiver, new BufferSenseSightActive(source));
                remembers.Remove(source);
            }
        }
    }
}