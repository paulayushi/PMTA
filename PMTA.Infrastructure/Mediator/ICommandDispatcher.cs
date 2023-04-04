using PMTA.Core.Command;

namespace PMTA.Infrastructure.Mediator
{
    public interface ICommandDispatcher
    {
        void RegisterHandler<T>(Func<T, Task> handler) where T:BaseCommand;
        Task SendAsync(BaseCommand command);
    }
}
