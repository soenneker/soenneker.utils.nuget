using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

public record NuGetRepository
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("branch")]
    public string? Branch { get; set; }

    [JsonPropertyName("commit")]
    public string? Commit { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
