using Unity.Entities;

namespace ECSPerception.Groups
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GroupSenses : ComponentSystemGroup
    {
    }
}