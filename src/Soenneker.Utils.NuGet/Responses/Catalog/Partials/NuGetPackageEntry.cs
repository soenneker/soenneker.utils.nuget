using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

/// <summary>
/// Represents the nu get package entry record.
/// </summary>
public record NuGetPackageEntry
{
    /// <summary>
    /// Gets or sets id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets type.
    /// </summary>
    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets compressed length.
    /// </summary>
    [JsonPropertyName("compressedLength")]
    public long? CompressedLength { get; set; }

    /// <summary>
    /// Gets or sets full name.
    /// </summary>
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets length.
    /// </summary>
    [JsonPropertyName("length")]
    public long? Length { get; set; }

    /// <summary>
    /// Gets or sets name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}