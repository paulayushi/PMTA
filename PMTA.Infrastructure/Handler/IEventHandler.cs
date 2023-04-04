using PMTA.Domain.Event;

namespace PMTA.Infrastructure.Handler
{
    public interface IEventHandler
    {
        Task On(MemberCreatedEvent memberCreatedEvent);
        Task On(MemberUpdatedEvent memberUpdatedEvent);
        Task On(TaskCreatedEvent taskCreatedEvent);
    }
}
