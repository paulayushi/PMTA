using PMTA.Domain.Entity;

namespace PMTA.Infrastructure.Repository
{
    public interface IMemberRepository
    {
        Task AddAsync(MemberEntity member);
        Task UpdateAsync(MemberEntity member);
        Task DeleteAsync(int memberId);
        Task<MemberEntity> GetByIdAsync(int memberId);
        Task<List<MemberEntity>> GetAllAsync();
    }
}
