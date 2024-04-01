using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetPackageTypeResponse
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}