namespace PMTA.Domain.DTO
{
    public class TaskDto
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public string? Delivarables { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskEndDate { get; set; }
    }
}
