using PMTA.Domain.DTO;
using PMTA.Domain.Entity;
using PMTA.Domain.Query;
using PMTA.Infrastructure.Repository;

namespace PMTA.WebAPI.Query
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ITaskRepository _taskRepository;
        public QueryHandler(IMemberRepository memberRepository, ITaskRepository taskRepository)
        {
            _memberRepository = memberRepository;
            _taskRepository = taskRepository;
        }

        public async Task<List<MemberEntity>> HandleAsync(GetAllMemberQuery query)
        {
            return await _memberRepository.GetAllAsync();
        }

        public async Task<List<TaskEntity>> HandleAsync(GetTasksByMemberIdQuery query)
        {
            return await _taskRepository.GetByMemberIdAsync(query.MemberId);
        }
    }
}
