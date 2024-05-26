using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PerceptionECS
{
    public sealed class SightSenseQueryAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _observer = null;
        [SerializeField] private GameObject _target = null;

        private class Baker : Baker<SightSenseQueryAuthoring>
        {
            public override void Bake(SightSenseQueryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SightSenseQueryComponent
                {
                    Observer = GetEntity(authoring._observer, TransformUsageFlags.None),
                    Target = GetEntity(authoring._target, TransformUsageFlags.None),
                    TargetPosition = float3.zero,
                });
                AddComponent(entity, new SightSenseVisibilityTag());
                SetComponentEnabled<SightSenseVisibilityTag>(entity, false);
            }
        }
    }

    public struct SightSenseQueryComponent : IComponentData
    {
        public Entity Observer;
        public Entity Target;
        public float3 TargetPosition;

        public void Deconstruct(out Entity observer, out Entity target, out float3 targetPosition)
        {
            observer = Observer;
            target = Target;
            targetPosition = TargetPosition;
        }
    }

    public struct SightSenseVisibilityTag : IComponentData, IEnableableComponent
    {
    }
}