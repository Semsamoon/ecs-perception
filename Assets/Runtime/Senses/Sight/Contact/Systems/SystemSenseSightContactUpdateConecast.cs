using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateContact), OrderFirst = true)]
    public partial struct SystemSenseSightContactUpdateConecast : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact>()
                         .WithDisabled<TagSenseSightContactConecastResult>().WithEntityAccess())
            {
                if (IsInCone(ref state, contact, false))
                {
                    commands.SetComponentEnabled<TagSenseSightContactConecastResult>(entity, true);
                }
            }

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseSightContactConecastResult>().WithEntityAccess())
            {
                if (!IsInCone(ref state, contact, true))
                {
                    commands.SetComponentEnabled<TagSenseSightContactConecastResult>(entity, false);
                }
            }

            commands.Playback(state.EntityManager);
        }

        [BurstCompile]
        private bool IsInCone(ref SystemState state, RefRO<ComponentSenseContact> contact, bool isExtendToLoseRadius)
        {
            var (entityReceiver, entitySource) = contact.ValueRO;

            var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
            var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

            var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
            var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
            var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

            var receiverPosition = receiverTransform.Position + receiverTransform.Forward * -receiver.BackwardOffset;
            var difference = sourceTransform.Position - receiverPosition;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!isExtendToLoseRadius ? receiver.ViewRadiusSquared : receiver.LoseRadiusSquared)
                || distanceSquared < receiver.NearClipRadiusSquared)
            {
                return false;
            }

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(receiverTransform.Forward, direction) >= receiver.ViewAngleCos;
        }
    }
}