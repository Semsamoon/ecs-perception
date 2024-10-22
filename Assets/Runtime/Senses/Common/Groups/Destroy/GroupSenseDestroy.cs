using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GroupSenseDestroy : ComponentSystemGroup
    {
    }
}