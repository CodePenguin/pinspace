using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinspace.Config
{
    public class PinPanelConfigConverter : JsonConverter<PinPanelConfig>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(PinPanelConfig).IsAssignableFrom(typeToConvert);
        }

        public override PinPanelConfig Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            var typeName = "Pinspace.Config." + reader.GetString() + "Config";
            var type = Type.GetType(typeName);
            if (!reader.Read() || reader.GetString() != "Properties")
            {
                throw new JsonException();
            }
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var config = JsonSerializer.Deserialize(ref reader, type) as PinPanelConfig;

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return config;
        }

        public override void Write(Utf8JsonWriter writer, PinPanelConfig value, JsonSerializerOptions options)
        {
            var typeName = value.GetType().Name.Replace("Config", "");
            writer.WriteStartObject();
            writer.WriteString("Type", typeName);
            if (value is FileListPinPanelConfig fileListPanelConfig)
            {
                writer.WritePropertyName("Properties");
                JsonSerializer.Serialize(writer, fileListPanelConfig);
            }
            else if (value is TextBoxPinPanelConfig textBoxPinPanelConfig)
            {
                writer.WritePropertyName("Properties");
                JsonSerializer.Serialize(writer, textBoxPinPanelConfig);
            }
            writer.WriteEndObject();
        }
    }
}
