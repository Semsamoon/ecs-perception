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
                    RememberTime = 0,
                });
                AddComponent(entity, new SightSenseVisibilityTag());
                AddComponent(entity, new SightSenseRememberTag());
                SetComponentEnabled<SightSenseVisibilityTag>(entity, false);
                SetComponentEnabled<SightSenseRememberTag>(entity, false);
            }
        }
    }

    public struct SightSenseQueryComponent : IComponentData
    {
        public Entity Observer;
        public Entity Target;
        public float3 TargetPosition;
        public float RememberTime;

        public void Deconstruct(out Entity observer, out Entity target, out float3 targetPosition, out float rememberTime)
        {
            observer = Observer;
            target = Target;
            targetPosition = TargetPosition;
            rememberTime = RememberTime;
        }
    }

    public struct SightSenseVisibilityTag : IComponentData, IEnableableComponent
    {
    }

    public struct SightSenseRememberTag : IComponentData, IEnableableComponent
    {
    }
}