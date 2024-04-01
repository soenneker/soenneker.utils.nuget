using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Utils.NuGet.Responses.Partials;

namespace Soenneker.Utils.NuGet.Responses;

public record NuGetIndexResponse
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("resources")]
    public List<NuGetResourceResponse>? Resources { get; set; }

    [JsonPropertyName("@context")]
    public NuGetContextResponse? Context { get; set; }
}