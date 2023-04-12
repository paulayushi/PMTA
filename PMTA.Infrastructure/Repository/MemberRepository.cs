using Microsoft.EntityFrameworkCore;
using PMTA.Infrastructure.DataAccess;
using MemberEntity = PMTA.Domain.Entity.MemberEntity;

namespace PMTA.Infrastructure.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly DbContextFactory _dbContextFactory;

        public MemberRepository(DbContextFactory dbContextFactory)
        {
            _dbContextFactory= dbContextFactory;
        }
        public async Task AddAsync(MemberEntity member)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Members.Add(member);
            _ = await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int memberId)
        {
            var member = await GetByIdAsync(memberId);
            
            if (member is null) return;

            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Members.Remove(member);
            _ = await dbContext.SaveChangesAsync();
        }

        public async Task<List<MemberEntity>> GetAllAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Members.AsNoTracking().Include(t => t.Tasks).ToListAsync();
        }

        public async Task<MemberEntity> GetByIdAsync(int memberId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Members.AsNoTracking().Include(t => t.Tasks).FirstOrDefaultAsync(x => x.MemberId == memberId);
        }

        public async Task UpdateAsync(MemberEntity member)
        {
            var memberFromDB = await GetByIdAsync(member.MemberId);
            if (memberFromDB is null) return;

            using var dbContext = _dbContextFactory.CreateDbContext();
            memberFromDB.AllocationPercentage = member.AllocationPercentage;
            dbContext.Update(memberFromDB);
            _ = await dbContext.SaveChangesAsync();

        }
        public async Task<MemberEntity> Login(int userId, string password)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var user = await dbContext.Members.FirstOrDefaultAsync(x => x.MemberId == userId);
            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        //public async Task<bool> UserExist(int userId)
        //{
        //    using var dbContext = _dbContextFactory.CreateDbContext();
        //    if (await dbContext.Members.AnyAsync(x => x.MemberId == userId))
        //        return true;
        //    return false;
        //}

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
