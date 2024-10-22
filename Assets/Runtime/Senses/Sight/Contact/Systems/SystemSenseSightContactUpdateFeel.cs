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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn>()
                         .WithDisabled<TagSenseContactFeel, TagSenseContactLineCast, TagSenseContactLineUp>().WithEntityAccess())
            {
                Linecast(ref commands, entity);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn, TagSenseContactLineUp>()
                         .WithDisabled<TagSenseContactFeel>().WithEntityAccess())
            {
                Linecast(ref commands, entity);
                SetFeel(ref commands, entity, true);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn, TagSenseContactLineUp>()
                         .WithAll<TagSenseContactFeel>().WithEntityAccess())
            {
                Linecast(ref commands, entity);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseSightContactConeIn, TagSenseContactFeel>()
                         .WithDisabled<TagSenseContactLineCast, TagSenseContactLineUp>().WithEntityAccess())
            {
                Linecast(ref commands, entity);
                SetFeel(ref commands, entity, false);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact, TagSenseContactFeel>()
                         .WithDisabled<TagSenseSightContactConeIn>().WithEntityAccess())
            {
                SetFeel(ref commands, entity, false);
            }

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseContactLineUp>().WithEntityAccess())
            {
                commands.SetComponentEnabled<TagSenseContactLineUp>(entity, false);
            }

            commands.Playback(state.EntityManager);
        }

        [BurstCompile]
        private void Linecast(ref EntityCommandBuffer commands, Entity entity)
        {
            commands.SetComponentEnabled<TagSenseContactLineCast>(entity, true);

            var eventLinecastCreate = commands.CreateEntity();
            commands.AddComponent(eventLinecastCreate, new EventSenseLinecastCreate
            {
                Contact = entity,
            });
            commands.AddComponent(eventLinecastCreate, new EventSenseSightLinecastCreate());
        }

        [BurstCompile]
        private void SetFeel(ref EntityCommandBuffer commands, Entity entity, bool isFeel)
        {
            commands.SetComponentEnabled<TagSenseContactFeel>(entity, isFeel);

            var eventUpdate = commands.CreateEntity();
            commands.AddComponent(eventUpdate, new EventSenseContactUpdateFeel
            {
                Entity = entity,
            });
            commands.AddComponent(eventUpdate, new EventSenseSightContactUpdateFeel());
        }
    }
}