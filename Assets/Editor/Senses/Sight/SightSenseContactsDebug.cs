using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECSPerception.Editor
{
    public sealed class SightSenseContactsDebug : MonoBehaviour
    {
        private EntityManager _manager;
        private EntityQuery _contactFeelQuery;
        private EntityQuery _contactRememberQuery;

        private void Start()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var builder = new EntityQueryBuilder(Allocator.Temp);

            _contactFeelQuery = builder.WithAll<ComponentSenseContact, TagSenseContactFeel>().Build(_manager);
            _contactRememberQuery = builder.Reset().WithAll<ComponentSenseContactRemember, TagSenseContactFeelRemember>()
                .WithDisabled<TagSenseContactFeel>().Build(_manager);
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