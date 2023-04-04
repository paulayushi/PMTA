using PMTA.Core.Command;
using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.Command
{
    public class UpdateMemberCommand: BaseCommand
    {
        [Required]
        public int MemberId { get; set; }
        public int AllocationPercentage { get; set; }
        public DateTime? ProjectEndDate { get; set; }
    }
}
