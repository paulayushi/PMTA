using PMTA.Domain.Entity;
using PMTA.Domain.Query;

namespace PMTA.WebAPI.Query
{
    public interface IQueryHandler
    {
        Task<List<MemberEntity>> HandleAsync(GetAllMemberQuery query);
        Task<List<TaskEntity>> HandleAsync(GetTasksByMemberIdQuery query);
    }
}
