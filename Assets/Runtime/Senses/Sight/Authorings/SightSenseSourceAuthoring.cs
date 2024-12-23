using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECSPerception.Sight
{
    public sealed class SightSenseSourceAuthoring : MonoBehaviour
    {
        [SerializeField] private float3 _offset = float3.zero;

        private sealed class Baker : Baker<SightSenseSourceAuthoring>
        {
            public override void Bake(SightSenseSourceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ComponentSenseSightSource
                {
                    Offset = authoring._offset,
                });
            }
        }
    }
}