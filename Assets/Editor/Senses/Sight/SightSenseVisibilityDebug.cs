using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
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

            _interactionFeel = _manager.CreateEntityQuery(typeof(ComponentSenseInteraction), typeof(TagSenseFeel));
            _interactionRemember = _manager.CreateEntityQuery(typeof(ComponentSenseRemember), typeof(TagSenseRemember));
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Gizmos.color = Color.green;

            foreach (var interaction in
                     _interactionFeel.ToComponentDataArray<ComponentSenseInteraction>(Allocator.Temp))
            {
                Gizmos.DrawWireSphere(_manager.GetComponentData<LocalToWorld>(interaction.Source).Position, 0.5f);
            }

            Gizmos.color = Color.yellow;

            foreach (var remember in
                     _interactionRemember.ToComponentDataArray<ComponentSenseRemember>(Allocator.Temp))
            {
                Gizmos.DrawWireSphere(remember.SourceTransform.Position, 0.5f);
            }
        }
    }
}