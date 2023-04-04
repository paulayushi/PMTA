using PMTA.Core.Event;
using PMTA.Domain.Event;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PMTA.Infrastructure.Converter
{
    public class EventJsonConverter : JsonConverter<BaseEvent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        }
        public override BaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader, out JsonDocument doc))
            {
                throw new JsonException($"Failed to parse {nameof(JsonDocument)}");
            }

            if (!doc.RootElement.TryGetProperty("Type", out var type))
            {
                throw new JsonException($"Could not detect the type discriminator property.");
            }

            var typeDiscriminator = type.GetString();
            var jsonDocument = doc.RootElement.GetRawText();

            return typeDiscriminator switch
            {
                nameof(MemberCreatedEvent) => JsonSerializer.Deserialize<MemberCreatedEvent>(jsonDocument, options),
                nameof(MemberUpdatedEvent) => JsonSerializer.Deserialize<MemberUpdatedEvent>(jsonDocument, options),
                nameof(TaskCreatedEvent) => JsonSerializer.Deserialize<TaskCreatedEvent>(jsonDocument, options),
                _ => throw new JsonException($"{typeDiscriminator} is not supported.")
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
