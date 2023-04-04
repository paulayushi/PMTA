using PMTA.Domain.Aggregate;

namespace PMTA.Common.Handler
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregate);
        Task<T> GetByIdAsync(Guid aggregateId);
    }
}
