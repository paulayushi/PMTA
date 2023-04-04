using PMTA.Core.Event;
using PMTA.Domain.Event;

namespace PMTA.Infrastructure.Store
{
    public interface IEventStore
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
        Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
    }
}
