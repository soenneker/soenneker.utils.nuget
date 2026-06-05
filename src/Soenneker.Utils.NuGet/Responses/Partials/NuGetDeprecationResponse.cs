using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get deprecation response record.
/// </summary>
public record NuGetDeprecationResponse
{
    /// <summary>
    /// Gets or sets alternate package.
    /// </summary>
    [JsonPropertyName("alternatePackage")]
    public NuGetAlternatePackageResponse? AlternatePackage { get; set; }

    /// <summary>
    /// Gets or sets reasons.
    /// </summary>
    [JsonPropertyName("reasons")]
    public List<string>? Reasons { get; set; }
}