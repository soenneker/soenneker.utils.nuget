using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

public record NuGetDependency
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public string? DependencyId { get; set; }

    [JsonPropertyName("range")]
    public string? Range { get; set; }
}