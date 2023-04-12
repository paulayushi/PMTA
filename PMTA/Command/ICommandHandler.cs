using PMTA.Domain.Command;
using PMTA.Domain.Entity;

namespace PMTA.WebAPI.Command
{
    public interface ICommandHandler
    {
        Task HandleAsync(CreateMemberCommand createMember);
        Task HandleAsync(UpdateMemberCommand updateMember);
        Task HandleAsync(CreateTaskCommand createTask);
        Task HandleAsync(CreateUserCommand createUser);
    }
}
