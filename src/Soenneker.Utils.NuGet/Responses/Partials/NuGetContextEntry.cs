using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetContextEntry
{
    [JsonPropertyName("@type")]
    public string? Type { get; set; }
}