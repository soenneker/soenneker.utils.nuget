using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetAlternatePackageResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("range")]
    public string? Range { get; set; }
}