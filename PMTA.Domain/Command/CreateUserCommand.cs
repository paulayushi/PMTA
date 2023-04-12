using PMTA.Core.Command;
using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.Command
{
    public class CreateUserCommand: BaseCommand
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 4, ErrorMessage = "Password should be atleast 4 characters of length!")]
        public string Password { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        [Required]
        public bool IsManager { get; set; }
    }
}
