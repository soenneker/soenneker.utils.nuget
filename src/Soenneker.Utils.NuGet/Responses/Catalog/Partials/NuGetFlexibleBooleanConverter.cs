using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

/// <summary>
/// Represents the nu get flexible boolean converter.
/// </summary>
public sealed class NuGetFlexibleBooleanConverter : JsonConverter<bool?>
{
    /// <summary>
    /// Executes the read operation.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The options.</param>
    /// <returns>The result of the operation.</returns>
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

    /// <summary>
    /// Executes the write operation.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="options">The options.</param>
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
