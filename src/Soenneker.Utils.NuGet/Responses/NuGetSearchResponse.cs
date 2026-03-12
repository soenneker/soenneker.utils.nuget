using Soenneker.Utils.NuGet.Responses.Partials;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses;

public record NuGetSearchResponse
{
    [JsonPropertyName("@context")]
    public NuGetContextResponse? Context { get; set; }

    [JsonPropertyName("totalHits")]
    public int TotalHits { get; set; }

    [JsonPropertyName("data")]
    public List<NuGetDataResponse>? Data { get; set; }
}