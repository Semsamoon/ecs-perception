using ECSPerception.Sight;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECSPerception.Editor.Sight
{
    public sealed class SightSenseContactsDebug : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _marksRadius = 0.5f;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Gizmos.color = Color.green;

            foreach (var receiver in new EntityQueryBuilder(Allocator.Temp)
                         .WithAll<BufferSenseSightActive>()
                         .Build(entityManager)
                         .ToEntityArray(Allocator.Temp))
            {
                var receiverData = entityManager.GetComponentData<ECSPerception.Sight.ComponentSenseSightReceiver>(receiver);
                var receiverTransform = entityManager.GetComponentData<LocalToWorld>(receiver);
                var receiverPosition = receiverTransform.Value.TransformPoint(receiverData.Offset);
                var actives = entityManager.GetBuffer<BufferSenseSightActive>(receiver);

                foreach (var contact in actives)
                {
                    var sourceData = entityManager.GetComponentData<ECSPerception.Sight.ComponentSenseSightSource>(contact.Source);
                    var sourceTransform = entityManager.GetComponentData<LocalToWorld>(contact.Source);
                    var sourcePosition = sourceTransform.Value.TransformPoint(sourceData.Offset);

                    Gizmos.DrawLine(receiverPosition, sourcePosition);
                    Gizmos.DrawWireSphere(sourcePosition, _marksRadius);
                }
            }

            Gizmos.color = Color.yellow;

            foreach (var receiver in new EntityQueryBuilder(Allocator.Temp)
                         .WithAll<BufferSenseSightRemember>()
                         .Build(entityManager)
                         .ToEntityArray(Allocator.Temp))
            {
                var receiverData = entityManager.GetComponentData<ECSPerception.Sight.ComponentSenseSightReceiver>(receiver);
                var receiverTransform = entityManager.GetComponentData<LocalToWorld>(receiver);
                var receiverPosition = receiverTransform.Value.TransformPoint(receiverData.Offset);
                var remembers = entityManager.GetBuffer<BufferSenseSightRemember>(receiver);

                foreach (var contact in remembers)
                {
                    var sourceData = entityManager.GetComponentData<ECSPerception.Sight.ComponentSenseSightSource>(contact.Source);
                    var sourceTransform = entityManager.GetComponentData<LocalToWorld>(contact.Source);
                    var sourcePosition = sourceTransform.Value.TransformPoint(sourceData.Offset);

                    Gizmos.DrawLine(receiverPosition, sourcePosition);
                    Gizmos.DrawWireSphere(sourcePosition, _marksRadius);
                }
            }
        }
    }
}