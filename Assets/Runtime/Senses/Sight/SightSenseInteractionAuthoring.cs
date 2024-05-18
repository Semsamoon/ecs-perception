using Unity.Entities;
using UnityEngine;

namespace PerceptionECS
{
    public sealed class SightSenseInteractionAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _observer = null;
        [SerializeField] private GameObject _target = null;

        private class Baker : Baker<SightSenseInteractionAuthoring>
        {
            public override void Bake(SightSenseInteractionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SightSenseInteractionComponent
                {
                    Observer = GetEntity(authoring._observer, TransformUsageFlags.None),
                    Target = GetEntity(authoring._target, TransformUsageFlags.None),
                });
                AddComponent(entity, new SightVisibilityTag());
                SetComponentEnabled<SightVisibilityTag>(entity, false);
            }
        }
    }

    public struct SightSenseInteractionComponent : IComponentData
    {
        public Entity Observer;
        public Entity Target;

        public void Deconstruct(out Entity observer, out Entity target)
        {
            observer = Observer;
            target = Target;
        }
    }

    public struct SightVisibilityTag : IComponentData, IEnableableComponent
    {
    }
}