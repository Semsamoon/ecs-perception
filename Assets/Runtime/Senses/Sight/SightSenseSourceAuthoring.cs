using Unity.Entities;
using UnityEngine;

namespace PerceptionECS
{
    public sealed class SightSenseSourceAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SightSenseSourceAuthoring>
        {
            public override void Bake(SightSenseSourceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SightSenseSourceTag());
            }
        }
    }

    public struct SightSenseSourceTag : IComponentData
    {
    }
}