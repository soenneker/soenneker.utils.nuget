using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetContextResponse
{
    [JsonPropertyName("@vocab")]
    public string? Vocab { get; set; }

    [JsonPropertyName("@base")]
    public string? Base { get; set; }
}