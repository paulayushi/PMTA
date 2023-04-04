using PMTA.Core.Handler;
using PMTA.Domain.Aggregate;
using PMTA.Domain.Command;

namespace PMTA.WebAPI.Command
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourcingHandler<MemberAggregate> _eventSourcingHandler;

        public CommandHandler(IEventSourcingHandler<MemberAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task HandleAsync(CreateMemberCommand createMember)
        {
            var aggregate = new MemberAggregate(createMember);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(UpdateMemberCommand updateMember)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(updateMember.Id);
            aggregate.UpdateAllocationPercentage(updateMember);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(CreateTaskCommand createTask)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(createTask.Id);
            aggregate.AddTask(createTask);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }
    }
}
