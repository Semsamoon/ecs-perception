using Unity.Mathematics;
using UnityEngine;

namespace ECSPerception.Editor
{
    public static class ExtendedDebug
    {
        public static void DrawOctahedron(float3 position, float3 size, Color color)
        {
            var upPosition = position + new float3(0, size.y / 2, 0);
            var downPosition = position + new float3(0, -size.y / 2, 0);

            var firstPosition = position + new float3(size.x / 2, 0, size.z / 2);
            var secondPosition = position + new float3(-size.x / 2, 0, size.z / 2);
            var thirdPosition = position + new float3(-size.x / 2, 0, -size.z / 2);
            var fourthPosition = position + new float3(size.x / 2, 0, -size.z / 2);

            Debug.DrawLine(firstPosition, upPosition, color);
            Debug.DrawLine(secondPosition, upPosition, color);
            Debug.DrawLine(thirdPosition, upPosition, color);
            Debug.DrawLine(fourthPosition, upPosition, color);

            Debug.DrawLine(firstPosition, downPosition, color);
            Debug.DrawLine(secondPosition, downPosition, color);
            Debug.DrawLine(thirdPosition, downPosition, color);
            Debug.DrawLine(fourthPosition, downPosition, color);

            Debug.DrawLine(firstPosition, secondPosition, color);
            Debug.DrawLine(secondPosition, thirdPosition, color);
            Debug.DrawLine(thirdPosition, fourthPosition, color);
            Debug.DrawLine(fourthPosition, firstPosition, color);
        }

        public static void DrawCurve(float3 position, float3 forward, float3 right, float angleRadians, float radius, Color color, float step)
        {
            for (var angle = -angleRadians / 2; angle < angleRadians / 2; angle += step)
            {
                var nextAngle = math.min(angle + step, angleRadians / 2);

                var x = math.cos(angle);
                var y = math.sin(angle);
                var next_x = math.cos(nextAngle);
                var next_y = math.sin(nextAngle);

                var start = position + forward * (radius * x) + right * (radius * y);
                var end = position + forward * (radius * next_x) + right * (radius * next_y);

                Debug.DrawLine(start, end, color);
            }
        }
    }
}