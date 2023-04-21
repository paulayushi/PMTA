using PMTA.Core.Command;
using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.Command
{
    public class UpdateMemberCommand: BaseCommand
    {
        [Required]
        public int MemberId { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Allocation should be provided as percentage(0,100)")]
        public int AllocationPercentage { get; set; }
        public DateTime? ProjectEndDate { get; set; }
    }
}
