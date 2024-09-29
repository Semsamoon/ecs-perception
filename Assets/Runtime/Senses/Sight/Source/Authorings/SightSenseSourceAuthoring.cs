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
                var entity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(entity, new ComponentSenseBase
                {
                    Entity = GetEntity(authoring, TransformUsageFlags.Dynamic),
                });
                AddComponent(entity, new ComponentSenseSightSource());
                AddComponent<EventSenseCreate>(entity);
                AddComponent<EventSenseDestroy>(entity);
                SetComponentEnabled<EventSenseDestroy>(entity, false);
            }
        }
    }
}