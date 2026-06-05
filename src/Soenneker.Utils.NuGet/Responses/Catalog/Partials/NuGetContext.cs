using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

/// <summary>
/// Represents the nu get context record.
/// </summary>
public record NuGetContext
{
    /// <summary>
    /// Gets or sets vocab.
    /// </summary>
    [JsonPropertyName("@vocab")]
    public string? Vocab { get; set; }

    /// <summary>
    /// Gets or sets catalog.
    /// </summary>
    [JsonPropertyName("catalog")]
    public string? Catalog { get; set; }

    /// <summary>
    /// Gets or sets xsd.
    /// </summary>
    [JsonPropertyName("xsd")]
    public string? Xsd { get; set; }

    /// <summary>
    /// Gets or sets dependencies.
    /// </summary>
    [JsonPropertyName("dependencies")]
    public NuGetCatalogContextEntry? Dependencies { get; set; }

    /// <summary>
    /// Gets or sets dependency groups.
    /// </summary>
    [JsonPropertyName("dependencyGroups")]
    public NuGetCatalogContextEntry? DependencyGroups { get; set; }

    /// <summary>
    /// Gets or sets package entries.
    /// </summary>
    [JsonPropertyName("packageEntries")]
    public NuGetCatalogContextEntry? PackageEntries { get; set; }

    /// <summary>
    /// Gets or sets package types.
    /// </summary>
    [JsonPropertyName("packageTypes")]
    public NuGetCatalogContextEntry? PackageTypes { get; set; }

    /// <summary>
    /// Gets or sets supported frameworks.
    /// </summary>
    [JsonPropertyName("supportedFrameworks")]
    public NuGetCatalogContextEntry? SupportedFrameworks { get; set; }

    /// <summary>
    /// Gets or sets tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public NuGetCatalogContextEntry? Tags { get; set; }

    /// <summary>
    /// Gets or sets vulnerabilities.
    /// </summary>
    [JsonPropertyName("vulnerabilities")]
    public NuGetCatalogContextEntry? Vulnerabilities { get; set; }

    /// <summary>
    /// Gets or sets published.
    /// </summary>
    [JsonPropertyName("published")]
    public NuGetCatalogContextEntry? Published { get; set; }

    /// <summary>
    /// Gets or sets created.
    /// </summary>
    [JsonPropertyName("created")]
    public NuGetCatalogContextEntry? Created { get; set; }

    /// <summary>
    /// Gets or sets last edited.
    /// </summary>
    [JsonPropertyName("lastEdited")]
    public NuGetCatalogContextEntry? LastEdited { get; set; }

    /// <summary>
    /// Gets or sets catalog commit time stamp.
    /// </summary>
    [JsonPropertyName("catalog:commitTimeStamp")]
    public NuGetCatalogContextEntry? CatalogCommitTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets reasons.
    /// </summary>
    [JsonPropertyName("reasons")]
    public NuGetCatalogContextEntry? Reasons { get; set; }
}