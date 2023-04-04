using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PMTA.Domain.Entity
{
    public class TaskEntity
    {
        [Key]
        public Guid TaskId { get; set; }
        [Required]
        public string TaskName { get; set; }
        public string? Delivarables { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskEndDate { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        [JsonIgnore]
        public virtual MemberEntity Member { get; set; }
    }
}
