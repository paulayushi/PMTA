using PMTA.Core.Event;

namespace PMTA.Domain.Event
{
    public class TaskCreatedEvent: BaseEvent
    {
        public TaskCreatedEvent() : base(nameof(TaskCreatedEvent))
        {
        }
        public string TaskName { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string Delivarables { get; set; }
        public DateTime TaskStartDate { get; set; }
        public DateTime? TaskEndDate { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
