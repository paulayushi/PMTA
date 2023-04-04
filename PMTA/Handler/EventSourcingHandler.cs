using PMTA.Core.Aggregate;
using PMTA.Core.Handler;
using PMTA.Domain.Aggregate;
using PMTA.Infrastructure.Store;

namespace PMTA.WebAPI.Handler
{
    public class EventSourcingHandler : IEventSourcingHandler<MemberAggregate>
    {
        private readonly IEventStore _eventStore;

        public EventSourcingHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<MemberAggregate> GetByIdAsync(Guid aggregateId)
        {
            var aggregate = new MemberAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any()) return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommited();
        }

    }
}
