using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace PerceptionECS
{
    public sealed class SightTargetAuthoring : MonoBehaviour
    {
        private Entity _entity = Entity.Null;

        private class Baker : Baker<SightTargetAuthoring>
        {
            public override void Bake(SightTargetAuthoring authoring)
            {
                authoring._entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(authoring._entity, new SightTargetTag());
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_entity == Entity.Null) return;

            var world = World.DefaultGameObjectInjectionWorld;
            foreach (var interactionEntity in
                     world.EntityManager.CreateEntityQuery(
                         typeof(SightSenseInteractionComponent),
                         typeof(SightVisibilityTag)).ToEntityArray(Allocator.Temp))
            {
                var interaction = world.EntityManager.GetComponentData<SightSenseInteractionComponent>(interactionEntity);
                var localToWorld = world.EntityManager.GetComponentData<LocalToWorld>(interaction.Target);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(localToWorld.Position, 0.5f);
            }
        }
#endif
    }

    public struct SightTargetTag : IComponentData
    {
    }
}