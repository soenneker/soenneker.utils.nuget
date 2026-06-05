using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get alternate package response record.
/// </summary>
public record NuGetAlternatePackageResponse
{
    /// <summary>
    /// Gets or sets id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets range.
    /// </summary>
    [JsonPropertyName("range")]
    public string? Range { get; set; }
}