using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateContact), OrderFirst = true), UpdateAfter(typeof(SystemSenseSightContactUpdateCone))]
    public partial struct SystemSenseSightContactUpdateFeel : ISystem
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
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn>()
                         .WithDisabled<TagSenseContactFeel, TagSenseContactLineCast, TagSenseContactLineUp>().WithEntityAccess())
            {
                Linecast(ref buffer, entity);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn, TagSenseContactLineUp>()
                         .WithDisabled<TagSenseContactFeel>().WithEntityAccess())
            {
                Linecast(ref buffer, entity);
                SetFeel(ref buffer, entity, true);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn, TagSenseContactLineUp>()
                         .WithAll<TagSenseContactFeel>().WithEntityAccess())
            {
                Linecast(ref buffer, entity);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn, TagSenseContactFeel>()
                         .WithDisabled<TagSenseContactLineCast, TagSenseContactLineUp>().WithEntityAccess())
            {
                Linecast(ref buffer, entity);
                SetFeel(ref buffer, entity, false);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact, TagSenseContactFeel>()
                         .WithDisabled<TagSenseSightContactConeIn>().WithEntityAccess())
            {
                SetFeel(ref buffer, entity, false);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseContactLineUp>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseContactLineUp>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        private void Linecast(ref EntityCommandBuffer buffer, Entity entity)
        {
            buffer.SetComponentEnabled<TagSenseContactLineCast>(entity, true);

            var eventLinecastCreate = buffer.CreateEntity();
            buffer.AddComponent(eventLinecastCreate, new EventSenseLinecastCreate
            {
                Contact = entity,
            });
            buffer.AddComponent(eventLinecastCreate, new EventSenseSightLinecastCreate());
        }

        [BurstCompile]
        private void SetFeel(ref EntityCommandBuffer buffer, Entity entity, bool isFeel)
        {
            buffer.SetComponentEnabled<TagSenseContactFeel>(entity, isFeel);

            var eventUpdate = buffer.CreateEntity();
            buffer.AddComponent(eventUpdate, new EventSenseContactUpdateFeel
            {
                Entity = entity,
            });
            buffer.AddComponent(eventUpdate, new EventSenseSightContactUpdateFeel());
        }
    }
}