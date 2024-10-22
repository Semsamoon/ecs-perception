using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseCreate))]
    public partial class GroupSenseCreateReceiver : ComponentSystemGroup
    {
    }
}