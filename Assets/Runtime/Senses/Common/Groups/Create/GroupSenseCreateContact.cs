using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseCreate)), UpdateAfter(typeof(GroupSenseCreateReceiver)), UpdateAfter(typeof(GroupSenseCreateSource))]
    public partial class GroupSenseCreateContact : ComponentSystemGroup
    {
    }
}