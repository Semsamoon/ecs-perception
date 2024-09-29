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

            _interactionFeel = _manager.CreateEntityQuery(typeof(ComponentSenseContact), typeof(TagSenseContactFeel));
            _interactionRemember = _manager.CreateEntityQuery(typeof(ComponentSenseContactRemember), typeof(TagSenseContactRemember));
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Gizmos.color = Color.green;

            foreach (var contact in
                     _interactionFeel.ToComponentDataArray<ComponentSenseContact>(Allocator.Temp))
            {
                var source = _manager.GetComponentData<ComponentSenseBase>(contact.Source).Entity;
                Gizmos.DrawWireSphere(_manager.GetComponentData<LocalToWorld>(source).Position, 0.5f);
            }

            Gizmos.color = Color.yellow;

            foreach (var remember in
                     _interactionRemember.ToComponentDataArray<ComponentSenseContactRemember>(Allocator.Temp))
            {
                Gizmos.DrawWireSphere(remember.SourceTransform.Position, 0.5f);
            }
        }
    }
}