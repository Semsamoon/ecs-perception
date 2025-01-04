using Unity.Entities;
using UnityEngine;

namespace ECSPerception.Editor.Sight
{
    public struct ComponentSenseSightReceiverDebug : IComponentData
    {
        public Color ColorNearClip;
        public Color ColorView;
        public Color ColorLose;

        public float CurveStep;

        public static ComponentSenseSightReceiverDebug Default => new()
        {
            ColorNearClip = Color.gray,
            ColorView = Color.green,
            ColorLose = Color.red,

            CurveStep = 0.02f,
        };
    }
}