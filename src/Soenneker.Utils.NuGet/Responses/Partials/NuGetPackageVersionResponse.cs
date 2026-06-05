using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get package version response record.
/// </summary>
public record NuGetPackageVersionResponse
{
    /// <summary>
    /// Gets or sets version number.
    /// </summary>
    [JsonPropertyName("version")]
    public string? VersionNumber { get; set; }

    /// <summary>
    /// Gets or sets downloads.
    /// </summary>
    [JsonPropertyName("downloads")]
    public int Downloads { get; set; }

    /// <summary>
    /// Gets or sets id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }
}