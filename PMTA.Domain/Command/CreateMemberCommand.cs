using PMTA.Core.Command;
using PMTA.Domain.CustomValidator;
using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.Command
{
    public class CreateMemberCommand : BaseCommand
    {
        [Required]
        public int MemberId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(5, int.MaxValue, ErrorMessage = "Experience must be greater than 4 years.")]
        public int Experience { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Member should possess atleast 3 skillsets.")]
        public string[] Skillset { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime ProjectStartDate { get; set; }
        [Required]
        [DateRangeValidator(nameof(ProjectStartDate), ErrorMessage = $"{nameof(ProjectEndDate)} must be greater than {nameof(ProjectStartDate)}")]
        public DateTime ProjectEndDate { get; set; }
        [Required]
        [Range(0,100, ErrorMessage = "Allocation should be provided as percentage.")]
        public int AllocationPercentage { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 4, ErrorMessage = "Password should be atleast 4 characters of length!")]
        public string Password { get; set; }
        public bool IsManager { get; set; } = false;
    }
}
