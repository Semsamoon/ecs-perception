using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(GroupSenseDestroy))]
    public partial class GroupSenseDestroySource : ComponentSystemGroup
    {
    }
}