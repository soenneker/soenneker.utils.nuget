using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetContextResponse
{
    [JsonPropertyName("@vocab")]
    public string? Vocab { get; set; }

    [JsonPropertyName("xsd")]
    public string? Xsd { get; set; }

    [JsonPropertyName("catalogEntry")]
    public NuGetContextEntry? CatalogEntry { get; set; }

    [JsonPropertyName("registration")]
    public NuGetContextEntry? Registration { get; set; }

    [JsonPropertyName("packageContent")]
    public NuGetContextEntry? PackageContent { get; set; }

    [JsonPropertyName("published")]
    public NuGetContextEntry? Published { get; set; }
}