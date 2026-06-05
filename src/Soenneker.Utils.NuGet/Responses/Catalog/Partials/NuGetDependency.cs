using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

/// <summary>
/// Represents the nu get dependency record.
/// </summary>
public record NuGetDependency
{
    /// <summary>
    /// Gets or sets id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets type.
    /// </summary>
    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets dependency id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? DependencyId { get; set; }

    /// <summary>
    /// Gets or sets range.
    /// </summary>
    [JsonPropertyName("range")]
    public string? Range { get; set; }
}