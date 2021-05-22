using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinspaces.Data
{
    public class PinJsonConverter : JsonConverter<Pin>
    {
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
            var typeName = "Pinspaces.Data." + reader.GetString();
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new ArgumentException($"Unknown Pin Type: {typeName}");
            }
            if (!reader.Read() || reader.GetString() != "Properties")
            {
                throw new JsonException();
            }
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var pin = JsonSerializer.Deserialize(ref reader, type) as Pin;

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return pin;
        }

        public override void Write(Utf8JsonWriter writer, Pin value, JsonSerializerOptions options)
        {
            var typeName = value.GetType().Name;
            writer.WriteStartObject();
            writer.WriteString("Type", typeName);
            if (value is FileListPin fileListPin)
            {
                writer.WritePropertyName("Properties");
                JsonSerializer.Serialize(writer, fileListPin);
            }
            else if (value is TextBoxPin textBoxPin)
            {
                writer.WritePropertyName("Properties");
                JsonSerializer.Serialize(writer, textBoxPin);
            }
            writer.WriteEndObject();
        }
    }
}
