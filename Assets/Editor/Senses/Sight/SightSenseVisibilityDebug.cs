using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PerceptionECS.Editor
{
    public sealed class SightSenseVisibilityDebug : MonoBehaviour
    {
        private EntityManager _manager;
        private EntityQuery _query;

        private void Start()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _query = _manager.CreateEntityQuery(
                ComponentType.ReadOnly<ComponentSenseInteractionRemember>(),
                ComponentType.ReadOnly<TagSenseFeel>());
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.green;

            foreach (var query in _query.ToComponentDataArray<ComponentSenseInteractionRemember>(Allocator.Temp))
            {
                Gizmos.DrawWireSphere(query.SourcePosition, 0.5f);
            }
        }
    }
}