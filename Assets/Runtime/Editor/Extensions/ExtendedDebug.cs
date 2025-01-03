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
    }
}