namespace PMTA.Domain.DTO
{
    public class MemberQueryResponse
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public string Skillset { get; set; }
        public string Description { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int AllocationPercentage { get; set; }
        public bool IsManager { get; set; }
        public List<TaskDto> Tasks { get; set; }
    }
}
