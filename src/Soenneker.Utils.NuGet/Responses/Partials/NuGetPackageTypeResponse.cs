using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get package type response record.
/// </summary>
public record NuGetPackageTypeResponse
{
    /// <summary>
    /// Gets or sets name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}