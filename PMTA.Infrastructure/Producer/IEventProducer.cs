using PMTA.Core.Event;

namespace PMTA.Infrastructure.Producer
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, T @event) where T:BaseEvent;
    }
}
