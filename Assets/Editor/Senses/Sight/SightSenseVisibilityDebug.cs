using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PerceptionECS.Editor
{
    public sealed class SightSenseVisibilityDebug : MonoBehaviour
    {
        private EntityManager _manager;
        private EntityQuery _interactionFeel;
        private EntityQuery _interactionRemember;

        private void Start()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _interactionFeel = _manager.CreateEntityQuery(
                ComponentType.ReadOnly<ComponentSenseInteractionRemember>(),
                ComponentType.ReadOnly<TagSenseFeel>());

            _interactionRemember = _manager.CreateEntityQuery(
                ComponentType.ReadOnly<ComponentSenseInteractionRemember>(),
                ComponentType.ReadOnly<TagSenseRemember>());
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.green;

            foreach (var remember in _interactionFeel.ToComponentDataArray<ComponentSenseInteractionRemember>(Allocator.Temp))
            {
                Gizmos.DrawWireSphere(remember.SourcePosition, 0.5f);
            }

            Gizmos.color = Color.yellow;

            foreach (var remember in _interactionRemember.ToComponentDataArray<ComponentSenseInteractionRemember>(Allocator.Temp))
            {
                Gizmos.DrawWireSphere(remember.SourcePosition, 0.5f);
            }
        }
    }
}