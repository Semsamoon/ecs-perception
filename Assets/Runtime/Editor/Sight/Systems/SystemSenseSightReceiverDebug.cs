using ECSPerception.Sight;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECSPerception.Editor.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup))]
    public partial struct SystemSenseSightReceiverDebug : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<ComponentSenseSightReceiverDebug>())
            {
                state.EntityManager.CreateSingleton(ComponentSenseSightReceiverDebug.Default);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var debug = SystemAPI.GetSingleton<ComponentSenseSightReceiverDebug>();

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

                    Debug.DrawLine(startPoint, nearClipPoint1, debug.ColorNearClip);
                    Debug.DrawLine(startPoint, nearClipPoint2, debug.ColorNearClip);
                    Debug.DrawLine(nearClipPoint1, viewPoint1, debug.ColorView);
                    Debug.DrawLine(nearClipPoint2, viewPoint2, debug.ColorView);
                    Debug.DrawLine(viewPoint1, losePoint1, debug.ColorLose);
                    Debug.DrawLine(viewPoint2, losePoint2, debug.ColorLose);
                }

                ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, nearClipRadius, debug.ColorView, debug.CurveStep);
                ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, viewRadius, debug.ColorView, debug.CurveStep);
                ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, loseRadius, debug.ColorLose, debug.CurveStep);
            }
        }
    }
}