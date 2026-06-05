using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

/// <summary>
/// Represents the nu get context mapping record.
/// </summary>
public record NuGetContextMapping
{
    /// <summary>
    /// Gets or sets id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets container.
    /// </summary>
    [JsonPropertyName("@container")]
    public string? Container { get; set; }
}