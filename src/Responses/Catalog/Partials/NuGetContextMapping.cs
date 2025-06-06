using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

public record NuGetContextMapping
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@container")]
    public string? Container { get; set; }
}