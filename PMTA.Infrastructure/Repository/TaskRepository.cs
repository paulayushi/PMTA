using Microsoft.EntityFrameworkCore;
using PMTA.Domain.Entity;
using PMTA.Infrastructure.DataAccess;

namespace PMTA.Infrastructure.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DbContextFactory _dbContextFactory;

        public TaskRepository(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task AddAsync(TaskEntity task)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Tasks.Add(task);
            _ = await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid taskId)
        {
            var task = await GetByIdAsync(taskId);

            if (task is null) return;

            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Tasks.Remove(task);
            _ = await dbContext.SaveChangesAsync();
        }

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task<TaskEntity> GetByIdAsync(Guid taskId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Tasks.FirstOrDefaultAsync(x => x.TaskId == taskId);
        }

        public async Task<List<TaskEntity>> GetByMemberIdAsync(int memberId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Tasks.Include(m => m.Member).Where(x => x.MemberId == memberId).ToListAsync();
        }

        public async Task UpdateAsync(TaskEntity task)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Update(task);
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
