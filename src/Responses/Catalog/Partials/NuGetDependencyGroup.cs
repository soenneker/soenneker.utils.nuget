using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

public record NuGetDependencyGroup
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    [JsonPropertyName("dependencies")]
    public List<NuGetDependency>? Dependencies { get; set; }

    [JsonPropertyName("targetFramework")]
    public string? TargetFramework { get; set; }
}