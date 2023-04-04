using PMTA.Core.Event;
using PMTA.Domain.Aggregate;
using PMTA.Domain.Event;
using PMTA.Infrastructure.Producer;
using PMTA.Infrastructure.Repository;

namespace PMTA.Infrastructure.Store
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _repository;
        private readonly IEventProducer _eventProducer;

        public EventStore(IEventStoreRepository repository, IEventProducer eventProducer)
        {
            _repository = repository;
            _eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _repository.FindByAggregateId(aggregateId);

            if (eventStream == null || !eventStream.Any())
                return null;
            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _repository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new Exception("Version incompatibility.");

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(MemberAggregate),
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };

                await _repository.SaveAsync(eventModel);

                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
