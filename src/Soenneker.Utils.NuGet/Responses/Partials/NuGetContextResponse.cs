using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get context response record.
/// </summary>
public record NuGetContextResponse
{
    /// <summary>
    /// Gets or sets vocab.
    /// </summary>
    [JsonPropertyName("@vocab")]
    public string? Vocab { get; set; }

    /// <summary>
    /// Gets or sets xsd.
    /// </summary>
    [JsonPropertyName("xsd")]
    public string? Xsd { get; set; }

    /// <summary>
    /// Gets or sets catalog entry.
    /// </summary>
    [JsonPropertyName("catalogEntry")]
    public NuGetContextEntry? CatalogEntry { get; set; }

    /// <summary>
    /// Gets or sets registration.
    /// </summary>
    [JsonPropertyName("registration")]
    public NuGetContextEntry? Registration { get; set; }

    /// <summary>
    /// Gets or sets package content.
    /// </summary>
    [JsonPropertyName("packageContent")]
    public NuGetContextEntry? PackageContent { get; set; }

    /// <summary>
    /// Gets or sets published.
    /// </summary>
    [JsonPropertyName("published")]
    public NuGetContextEntry? Published { get; set; }
}