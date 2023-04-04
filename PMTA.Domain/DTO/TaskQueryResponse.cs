namespace PMTA.Domain.DTO
{
    public class TaskQueryResponse
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public string? Delivarables { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskEndDate { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int AllocationPercentage { get; set; }
    }
}
