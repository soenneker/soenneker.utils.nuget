using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get context entry record.
/// </summary>
public record NuGetContextEntry
{
    /// <summary>
    /// Gets or sets type.
    /// </summary>
    [JsonPropertyName("@type")]
    public string? Type { get; set; }
}