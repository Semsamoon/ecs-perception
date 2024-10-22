using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(SimulationSystemGroup)), UpdateAfter(typeof(GroupSenseDestroy))]
    public partial class GroupSenseCreate : ComponentSystemGroup
    {
    }
}