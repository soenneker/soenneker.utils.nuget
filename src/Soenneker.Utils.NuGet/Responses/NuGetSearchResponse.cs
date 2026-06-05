using Soenneker.Utils.NuGet.Responses.Partials;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses;

/// <summary>
/// Represents the nu get search response record.
/// </summary>
public record NuGetSearchResponse
{
    /// <summary>
    /// Gets or sets context.
    /// </summary>
    [JsonPropertyName("@context")]
    public NuGetContextResponse? Context { get; set; }

    /// <summary>
    /// Gets or sets total hits.
    /// </summary>
    [JsonPropertyName("totalHits")]
    public int TotalHits { get; set; }

    /// <summary>
    /// Gets or sets data.
    /// </summary>
    [JsonPropertyName("data")]
    public List<NuGetDataResponse>? Data { get; set; }
}