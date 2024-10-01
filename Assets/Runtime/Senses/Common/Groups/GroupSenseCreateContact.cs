using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(GroupSenseCreate)), UpdateAfter(typeof(GroupSenseCreateReceiver)), UpdateAfter(typeof(GroupSenseCreateSource))]
    public partial class GroupSenseCreateContact : ComponentSystemGroup
    {
    }
}