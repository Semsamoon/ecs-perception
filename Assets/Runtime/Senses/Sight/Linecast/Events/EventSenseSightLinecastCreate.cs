﻿using Unity.Entities;

namespace ECSPerception
{
    public struct EventSenseSightLinecastCreate : IComponentData
    {
        public Entity Contact;
    }
}