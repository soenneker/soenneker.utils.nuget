using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

public sealed class NuGetFlexibleBooleanConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.True)
            return true;

        if (reader.TokenType == JsonTokenType.False)
            return false;

        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (bool.TryParse(value, out bool parsed))
                return parsed;
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when reading a flexible boolean value.");
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteBooleanValue(value.Value);
    }
}
