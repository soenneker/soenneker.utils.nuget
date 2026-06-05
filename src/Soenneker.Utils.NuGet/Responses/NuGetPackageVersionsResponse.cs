using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses;

/// <summary>
/// Represents the nu get package versions response record.
/// </summary>
public record NuGetPackageVersionsResponse
{
    /// <summary>
    /// Gets or sets versions.
    /// </summary>
    [JsonPropertyName("versions")]
    public List<string>? Versions { get; set; }
}