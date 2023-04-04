using PMTA.Core.Event;

namespace PMTA.Domain.Event
{
    public class MemberUpdatedEvent: BaseEvent
    {
        public MemberUpdatedEvent():base(nameof(MemberUpdatedEvent))
        {
        }
        public int MemberId { get; set; }
        public int AllocationPercentage { get; set; }
        public DateTime? ProjectEndDate { get; set; }
    }
}
