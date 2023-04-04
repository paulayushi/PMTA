using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PMTA.Core.Consumer;
using PMTA.Core.Event;
using PMTA.Infrastructure.Converter;
using PMTA.Infrastructure.Handler;
using System.Text.Json;

namespace PMTA.Infrastructure.Consumer
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _consumerCofig;
        private readonly IEventHandler _eventHandler;

        public EventConsumer(IOptions<ConsumerConfig> consumerCofig, IEventHandler eventHandler)
        {
            _consumerCofig = consumerCofig.Value;
            _eventHandler = eventHandler;
        }

        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerCofig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();


            consumer.Subscribe(topic);

            while (true)
            {
                var consumeResult = consumer.Consume();

                if (consumeResult.Message == null) continue;

                var options = new JsonSerializerOptions() { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);

                var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });
                if (handlerMethod is null)
                    throw new Exception($"{nameof(handlerMethod)} event handler method is not found.");

                handlerMethod.Invoke(_eventHandler, new object[] { @event });
                consumer.Commit();
            }

        }
    }
}
