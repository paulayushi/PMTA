using PMTA.Core.Command;
using PMTA.Domain.CustomValidator;
using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.Command
{
    public class CreateTaskCommand: BaseCommand
    {
        [Required]
        public string TaskName { get; set; }
        [Required]
        public int MemberId { get; set; }
        [Required]
        public string Delivarables { get; set; }
        [Required]
        public DateTime TaskStartDate { get; set; }
        [DateRangeValidator(nameof(TaskStartDate), ErrorMessage = $"{nameof(TaskEndDate)} must be greater than {nameof(TaskStartDate)}")]
        public DateTime? TaskEndDate { get; set; }
        public string? MemberName { get; set; }
    }
}
