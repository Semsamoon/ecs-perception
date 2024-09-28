using Unity.Entities;
using UnityEngine;

namespace PerceptionECS
{
    public sealed class SightSenseSourceAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<SightSenseSourceAuthoring>
        {
            public override void Bake(SightSenseSourceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<ComponentSenseSightSource>(entity);
                AddComponent<TagSenseRegisterSource>(entity);
                AddComponent<SightSenseSourceUnregisterTag>(entity);
                SetComponentEnabled<SightSenseSourceUnregisterTag>(entity, false);
            }
        }
    }
}