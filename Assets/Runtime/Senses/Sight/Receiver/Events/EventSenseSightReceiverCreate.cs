﻿using Unity.Entities;

namespace PerceptionECS
{
    public struct EventSenseSightReceiverCreate : IComponentData
    {
        public float ViewAngleCos;
        public float ViewRadiusSquared;
        public float LoseRadiusSquared;
        public float BackwardOffset;
        public float NearClipRadiusSquared;
    }
}