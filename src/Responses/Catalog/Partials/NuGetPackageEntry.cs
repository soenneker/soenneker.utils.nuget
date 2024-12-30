using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

public record NuGetPackageEntry
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    [JsonPropertyName("compressedLength")]
    public long? CompressedLength { get; set; }

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("length")]
    public long? Length { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}