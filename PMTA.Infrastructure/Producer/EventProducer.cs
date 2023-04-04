using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PMTA.Core.Event;
using System.Text.Json;

namespace PMTA.Infrastructure.Producer
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        public EventProducer(IOptions<ProducerConfig> config)
        {
            _config = config.Value;
        }

        public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
        {
            using var producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            var eventMessage = new Message<string, string>()
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };

            var produceResult = await producer.ProduceAsync(topic, @eventMessage);

            if (produceResult.Status == PersistenceStatus.NotPersisted)
                throw new Exception($"Could not produce {@event.GetType().Name} message to topic - {topic} " +
                    $"due to the following reason: {produceResult.Message}");

        }
    }
}
