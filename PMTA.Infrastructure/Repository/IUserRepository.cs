using PMTA.Domain.Entity;

namespace PMTA.Infrastructure.Repository
{
    public interface IUserRepository
    {
        Task RegisterAsync(UserEntity user, string password);
        Task<UserEntity> Login(string userName, string password);
        Task<bool> UserExist(string username);

    }
}
