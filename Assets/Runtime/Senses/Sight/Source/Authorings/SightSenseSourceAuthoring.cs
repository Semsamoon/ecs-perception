using Unity.Entities;
using UnityEngine;

namespace ECSPerception
{
    public sealed class SightSenseSourceAuthoring : MonoBehaviour
    {
        private sealed class Baker : Baker<SightSenseSourceAuthoring>
        {
            public override void Bake(SightSenseSourceAuthoring authoring)
            {
                var entity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(entity, new EventSenseSourceCreate
                {
                    Owner = GetEntity(TransformUsageFlags.Dynamic),
                    Transform = GetEntity(TransformUsageFlags.Dynamic),
                });
                AddComponent(entity, new EventSenseSightSourceCreate());
            }
        }
    }
}