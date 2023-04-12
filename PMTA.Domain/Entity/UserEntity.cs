using System.ComponentModel.DataAnnotations;

namespace PMTA.Domain.Entity
{
    public class UserEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public bool IsManager { get; set; }
    }
}
