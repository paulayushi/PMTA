using PMTA.Core.Event;

namespace PMTA.Domain.Event
{
    public class MemberCreatedEvent : BaseEvent
    {
        public MemberCreatedEvent() : base(nameof(MemberCreatedEvent))
        {
        }
        public int MemberId { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public string[] Skillset { get; set; }
        public string Description { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int AllocationPercentage { get; set; }
        public DateTime DateCreated { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public bool IsManager { get; set; } = false;
    }
}
