using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetPackageVersionResponse
{
    [JsonPropertyName("version")]
    public string? VersionNumber { get; set; }

    [JsonPropertyName("downloads")]
    public int Downloads { get; set; }

    [JsonPropertyName("@id")]
    public string? Id { get; set; }
}