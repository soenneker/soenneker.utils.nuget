using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

/// <summary>
/// Represents the nu get repository converter.
/// </summary>
public sealed class NuGetRepositoryConverter : JsonConverter<NuGetRepository?>
{
    /// <summary>
    /// Executes the read operation.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The options.</param>
    /// <returns>The result of the operation.</returns>
    public override NuGetRepository? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();
            return string.IsNullOrWhiteSpace(value) ? null : new NuGetRepository { Url = value };
        }

        if (reader.TokenType == JsonTokenType.StartObject)
            return JsonSerializer.Deserialize<NuGetRepository>(ref reader, options);

        throw new JsonException($"Unexpected token {reader.TokenType} when reading repository.");
    }

    /// <summary>
    /// Executes the write operation.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="options">The options.</param>
    public override void Write(Utf8JsonWriter writer, NuGetRepository? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value, options);
    }
}
