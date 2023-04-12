using Microsoft.EntityFrameworkCore;
using PMTA.Domain.Entity;
using PMTA.Infrastructure.DataAccess;

namespace PMTA.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContextFactory _dbContextFactory;

        public UserRepository(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task RegisterAsync(UserEntity user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Id= Guid.NewGuid();

            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Users.Add(user);
            _ = await dbContext.SaveChangesAsync();
        }

        public async Task<UserEntity> Login(string userName, string password)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == userName);
            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task<bool> UserExist(string username)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            if (await dbContext.Users.AnyAsync(x => x.Username == username))
                return true;
            return false;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hkey = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hkey.Key;
                passwordHash = hkey.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hkey = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedPassword = hkey.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedPassword.Length; i++)
                {
                    if (computedPassword[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
    }
}
