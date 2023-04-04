using PMTA.Domain.Command;

namespace PMTA.WebAPI.Command
{
    public interface ICommandHandler
    {
        Task HandleAsync(CreateMemberCommand createMember);
        Task HandleAsync(UpdateMemberCommand updateMember);
        Task HandleAsync(CreateTaskCommand createTask);
    }
}
