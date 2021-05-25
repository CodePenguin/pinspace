using Pinspaces.Core.Data;
using Pinspaces.Interfaces;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinspaces.Data
{
    public class PinJsonConverter : JsonConverter<Pin>
    {
        private readonly IPinFactory pinFactory;

        public PinJsonConverter(IPinFactory pinFactory)
        {
            this.pinFactory = pinFactory;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Pin).IsAssignableFrom(typeToConvert);
        }

        public override Pin Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "Type")
            {
                throw new NotImplementedException();
            }
            if (!reader.Read() || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            var pinTypeName = reader.GetString();
            var pinType = pinFactory.GetPinType(pinTypeName);
            if (!reader.Read() || reader.GetString() != "Properties")
            {
                throw new JsonException();
            }
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var pin = JsonSerializer.Deserialize(ref reader, pinType) as Pin;

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return pin;
        }

        public override void Write(Utf8JsonWriter writer, Pin value, JsonSerializerOptions options)
        {
            var typeName = value.GetType().FullName;
            writer.WriteStartObject();
            writer.WriteString("Type", typeName);
            writer.WritePropertyName("Properties");
            JsonSerializer.Serialize(writer, (object)value);
            writer.WriteEndObject();
        }
    }
}
