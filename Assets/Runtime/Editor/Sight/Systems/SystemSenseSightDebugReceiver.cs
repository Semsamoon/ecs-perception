using ECSPerception.Sight;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECSPerception.Editor.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderLast = true), UpdateAfter(typeof(SystemSenseSightActive))]
    public partial struct SystemSenseSightDebugReceiver : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var debug = SystemAPI.GetSingleton<ComponentSenseSightDebug>();

            foreach (var (receiverData, receiverTransform) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<TagSenseDebug>())
            {
                var forward = receiverTransform.ValueRO.Forward;
                var right = receiverTransform.ValueRO.Right;
                var angleRadians = math.acos(receiverData.ValueRO.ViewAngleCos) * 2;

                var x = receiverData.ValueRO.ViewAngleCos;
                var y = math.sqrt(1 - receiverData.ValueRO.ViewAngleCos * receiverData.ValueRO.ViewAngleCos);

                var nearClipRadius = math.sqrt(receiverData.ValueRO.NearClipRadiusSquared);
                var viewRadius = math.sqrt(receiverData.ValueRO.ViewRadiusSquared);
                var loseRadius = math.sqrt(receiverData.ValueRO.LoseRadiusSquared);
                var startPoint = receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset);

                if (receiverData.ValueRO.ViewAngleCos != -1)
                {
                    var nearClipPoint1 = startPoint + forward * x * nearClipRadius + right * y * nearClipRadius;
                    var nearClipPoint2 = startPoint + forward * x * nearClipRadius + right * -y * nearClipRadius;
                    var viewPoint1 = startPoint + forward * x * viewRadius + right * y * viewRadius;
                    var viewPoint2 = startPoint + forward * x * viewRadius + right * y * -viewRadius;
                    var losePoint1 = startPoint + forward * x * loseRadius + right * y * loseRadius;
                    var losePoint2 = startPoint + forward * x * loseRadius + right * y * -loseRadius;

                    Debug.DrawLine(startPoint, nearClipPoint1, debug.ReceiverNearClipColor);
                    Debug.DrawLine(startPoint, nearClipPoint2, debug.ReceiverNearClipColor);
                    Debug.DrawLine(nearClipPoint1, viewPoint1, debug.ReceiverViewColor);
                    Debug.DrawLine(nearClipPoint2, viewPoint2, debug.ReceiverViewColor);
                    Debug.DrawLine(viewPoint1, losePoint1, debug.ReceiverLoseColor);
                    Debug.DrawLine(viewPoint2, losePoint2, debug.ReceiverLoseColor);
                }

                ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, nearClipRadius, debug.ReceiverViewColor, debug.CurveStep);
                ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, viewRadius, debug.ReceiverViewColor, debug.CurveStep);
                ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, loseRadius, debug.ReceiverLoseColor, debug.CurveStep);
            }

            foreach (var (_, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>>()
                         .WithAll<BufferSenseSightActive>()
                         .WithAll<TagSenseDebug>()
                         .WithEntityAccess())
            {
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);

                foreach (var active in actives)
                {
                    ExtendedDebug.DrawOctahedron(active.SourcePosition, debug.SizeOctahedronStandard, debug.RaySuccessColor);
                }
            }

            foreach (var (_, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>>()
                         .WithAll<BufferSenseSightMemory>()
                         .WithAll<TagSenseDebug>()
                         .WithEntityAccess())
            {
                var memories = SystemAPI.GetBuffer<BufferSenseSightMemory>(receiver);

                foreach (var memory in memories)
                {
                    var offset = SystemAPI.GetComponent<ComponentSenseSightSource>(memory.Source).Offset;
                    var sourceCurrentPosition = SystemAPI.GetComponent<LocalToWorld>(memory.Source).Value.TransformPoint(offset);

                    ExtendedDebug.DrawOctahedron(memory.SourcePosition, debug.SizeOctahedronSmall, debug.RayNeutralColor);
                    ExtendedDebug.DrawOctahedron(sourceCurrentPosition, debug.SizeOctahedronStandard, debug.RayNeutralColor);
                    Debug.DrawLine(memory.SourcePosition, sourceCurrentPosition, debug.RayNeutralColor);
                }
            }
        }
    }
}