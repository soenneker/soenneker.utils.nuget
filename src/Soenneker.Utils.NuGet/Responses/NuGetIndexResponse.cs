using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Utils.NuGet.Responses.Partials;

namespace Soenneker.Utils.NuGet.Responses;

/// <summary>
/// Represents the nu get index response record.
/// </summary>
public record NuGetIndexResponse
{
    /// <summary>
    /// Gets or sets version.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets resources.
    /// </summary>
    [JsonPropertyName("resources")]
    public List<NuGetResourceResponse>? Resources { get; set; }

    /// <summary>
    /// Gets or sets context.
    /// </summary>
    [JsonPropertyName("@context")]
    public NuGetContextResponse? Context { get; set; }
}