using PMTA.Domain.Entity;

namespace PMTA.Infrastructure.Repository
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskEntity task);
        Task UpdateAsync(TaskEntity task);
        Task DeleteAsync(Guid taskId);
        Task<TaskEntity> GetByIdAsync(Guid taskId);
        Task<List<TaskEntity>> GetAllAsync();
        Task<List<TaskEntity>> GetByMemberIdAsync(int memberId);
    }
}
