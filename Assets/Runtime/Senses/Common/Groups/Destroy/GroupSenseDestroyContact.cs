using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseDestroy)), UpdateAfter(typeof(GroupSenseDestroyReceiver)), UpdateAfter(typeof(GroupSenseDestroySource))]
    public partial class GroupSenseDestroyContact : ComponentSystemGroup
    {
    }
}