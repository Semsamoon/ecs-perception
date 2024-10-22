using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseDestroy))]
    public partial class GroupSenseDestroyReceiver : ComponentSystemGroup
    {
    }
}