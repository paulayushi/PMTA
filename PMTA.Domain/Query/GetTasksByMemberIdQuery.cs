using PMTA.Core.Query;

namespace PMTA.Domain.Query
{
    public class GetTasksByMemberIdQuery: BaseQuery
    {
        public int MemberId { get; set; }
    }
}
