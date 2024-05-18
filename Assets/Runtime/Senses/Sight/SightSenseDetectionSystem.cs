using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

namespace PerceptionECS
{
    public partial struct SightSenseDetectionSystem : ISystem
    {
        private NativeList<RaycastHit> _hits;
        private CollisionWorld _collisionWorld;

        public void OnCreate(ref SystemState state)
        {
            _hits = new NativeList<RaycastHit>(4, Allocator.Persistent);
            _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        }

        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);
            _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (interaction, interactionEntity) in
                     SystemAPI.Query<RefRO<SightSenseInteractionComponent>>().WithDisabled<SightVisibilityTag>().WithEntityAccess())
            {
                var (observerEntity, targetEntity) = interaction.ValueRO;
                var observer = SystemAPI.GetComponentRO<LocalToWorld>(observerEntity);
                var sightSense = SystemAPI.GetComponentRO<SightSenseComponent>(observerEntity);
                var target = SystemAPI.GetComponentRO<LocalToWorld>(targetEntity);
                if (!IsInSightCone(observer.ValueRO, sightSense.ValueRO, target.ValueRO, false)) continue;
                if (!IsRayConnectsDirectly(observerEntity, observer.ValueRO, sightSense.ValueRO, targetEntity, target.ValueRO)) continue;

                buffer.SetComponentEnabled<SightVisibilityTag>(interactionEntity, true);
            }

            foreach (var (interaction, entity) in
                     SystemAPI.Query<RefRO<SightSenseInteractionComponent>>().WithAll<SightVisibilityTag>().WithEntityAccess())
            {
                var (observerEntity, targetEntity) = interaction.ValueRO;
                var observer = SystemAPI.GetComponentRO<LocalToWorld>(observerEntity);
                var sightSense = SystemAPI.GetComponentRO<SightSenseComponent>(observerEntity);
                var target = SystemAPI.GetComponentRO<LocalToWorld>(targetEntity);
                if (IsInSightCone(observer.ValueRO, sightSense.ValueRO, target.ValueRO, true)
                    && IsRayConnectsDirectly(observerEntity, observer.ValueRO, sightSense.ValueRO, targetEntity, target.ValueRO)) return;

                buffer.SetComponentEnabled<SightVisibilityTag>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }

        private bool IsInSightCone(
            in LocalToWorld observer, in SightSenseComponent sightSense, in LocalToWorld target, bool extendToLoseRadius)
        {
            var position = observer.Position + observer.Forward * -sightSense.BackwardOffset;
            var difference = target.Position - position;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!extendToLoseRadius ? sightSense.ViewRadiusSquared : sightSense.LoseRadiusSquared)
                || distanceSquared < sightSense.NearClipRadiusSquared) return false;

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(observer.Forward, direction) >= sightSense.ViewAngleCos;
        }

        private bool IsRayConnectsDirectly(
            Entity observerEntity, in LocalToWorld observer, in SightSenseComponent sightSense,
            Entity targetEntity, in LocalToWorld target)
        {
            var raycast = new RaycastInput
            {
                Start = observer.Position + observer.Forward * -sightSense.BackwardOffset,
                End = target.Position,
                Filter = CollisionFilter.Default,
            };
            _hits.Clear();
            _collisionWorld.CastRay(raycast, ref _hits);

            foreach (var hit in _hits)
            {
                if (hit.Entity != observerEntity && hit.Entity != targetEntity) return false;
            }

            return true;
        }
    }
}