using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTA.Domain.Entity
{
    public class MemberEntity
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MemberId { get; set; }
        public Guid EventId { get; set; }        
        [Required]
        public string Name { get; set; }
        [Required]
        public int Experience { get; set; }
        [Required]
        public string Skillset { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime ProjectStartDate { get; set; }
        [Required]
        public DateTime ProjectEndDate { get; set; }
        [Required]
        public int AllocationPercentage { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public bool IsManager { get; set; }
        public virtual ICollection<TaskEntity> Tasks { get; set; }
    }
}
