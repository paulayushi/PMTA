using PMTA.Core.Event;
using PMTA.Domain.Event;

namespace PMTA.Infrastructure.Repository
{
    public interface IEventStoreRepository
    {
        Task SaveAsync(EventModel @event);
        Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
    }
}
