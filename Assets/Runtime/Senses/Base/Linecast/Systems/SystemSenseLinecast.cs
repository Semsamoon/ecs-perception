﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateLinecast), OrderFirst = true)]
    public partial struct SystemSenseLinecast : ISystem
    {
        private float _timer;

        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (linecast, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseLinecast>>().WithAll<TagSenseLinecastWait>().WithEntityAccess())
            {
                var entityReceiverTransform = linecast.ValueRO.ReceiverTransform;
                var entityReceiverOwner = linecast.ValueRO.ReceiverOwner;
                var receiverOffset = linecast.ValueRO.ReceiverOffset;
                var entitySourceTransform = linecast.ValueRO.SourceTransform;
                var entitySourceOwner = linecast.ValueRO.SourceOwner;
                var sourceOffset = linecast.ValueRO.SourceOffset;

                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiverTransform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(entitySourceTransform).ValueRO;

                if (IsLineCastSucceed(entityReceiverOwner, receiverTransform.Position + receiverOffset,
                        entitySourceOwner, sourceTransform.Position + sourceOffset, ref collisionWorld))
                {
                    buffer.SetComponentEnabled<TagSenseLinecastSuccess>(entity, true);
                }

                buffer.SetComponentEnabled<TagSenseLinecastWait>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        private bool IsLineCastSucceed(
            Entity entityReceiverOwner, in float3 receiverPosition,
            Entity entitySourceOwner, in float3 sourcePosition, ref CollisionWorld collisionWorld)
        {
            var raycast = new RaycastInput
            {
                Start = receiverPosition,
                End = sourcePosition,
                Filter = CollisionFilter.Default,
            };

            var hits = new NativeList<RaycastHit>(4, Allocator.Temp);
            collisionWorld.CastRay(raycast, ref hits);

            foreach (var hit in hits)
            {
                if (hit.Entity != entityReceiverOwner && hit.Entity != entitySourceOwner)
                {
                    return false;
                }
            }

            return true;
        }
    }
}