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
                AddComponent<SightSenseSourceTag>(entity);
                AddComponent<SightSenseSourceRegisterTag>(entity);
                AddComponent<SightSenseSourceUnregisterTag>(entity);
                SetComponentEnabled<SightSenseSourceUnregisterTag>(entity, false);
            }
        }
    }
}