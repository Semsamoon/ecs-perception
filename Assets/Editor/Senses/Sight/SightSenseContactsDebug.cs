using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace PerceptionECS.Editor
{
    public sealed class SightSenseContactsDebug : MonoBehaviour
    {
        private EntityManager _manager;
        private EntityQuery _contactFeelQuery;
        private EntityQuery _contactRememberQuery;

        private void Start()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var contactFeelTypes = new ComponentType[]
            {
                typeof(ComponentSenseContact),
                typeof(TagSenseContactFeel),
            };
            _contactFeelQuery = _manager.CreateEntityQuery(contactFeelTypes);

            var contactRememberTypes = new ComponentType[]
            {
                typeof(ComponentSenseContactRemember),
                typeof(TagSenseContactRemember),
            };
            _contactRememberQuery = _manager.CreateEntityQuery(contactRememberTypes);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Gizmos.color = Color.green;

            foreach (var contact in _contactFeelQuery.ToComponentDataArray<ComponentSenseContact>(Allocator.Temp))
            {
                var source = _manager.GetComponentData<ComponentSenseSource>(contact.Source).Transform;
                Gizmos.DrawWireSphere(_manager.GetComponentData<LocalToWorld>(source).Position, 0.5f);
            }

            Gizmos.color = Color.yellow;

            foreach (var remember in _contactRememberQuery.ToComponentDataArray<ComponentSenseContactRemember>(Allocator.Temp))
            {
                Gizmos.DrawWireSphere(remember.SourceTransform.Position, 0.5f);
            }
        }
    }
}