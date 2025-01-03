using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

public record NuGetContext
{
    [JsonPropertyName("@vocab")]
    public string? Vocab { get; set; }

    [JsonPropertyName("catalog")]
    public string? Catalog { get; set; }

    [JsonPropertyName("xsd")]
    public string? Xsd { get; set; }

    [JsonPropertyName("dependencies")]
    public NuGetCatalogContextEntry? Dependencies { get; set; }

    [JsonPropertyName("dependencyGroups")]
    public NuGetCatalogContextEntry? DependencyGroups { get; set; }

    [JsonPropertyName("packageEntries")]
    public NuGetCatalogContextEntry? PackageEntries { get; set; }

    [JsonPropertyName("packageTypes")]
    public NuGetCatalogContextEntry? PackageTypes { get; set; }

    [JsonPropertyName("supportedFrameworks")]
    public NuGetCatalogContextEntry? SupportedFrameworks { get; set; }

    [JsonPropertyName("tags")]
    public NuGetCatalogContextEntry? Tags { get; set; }

    [JsonPropertyName("vulnerabilities")]
    public NuGetCatalogContextEntry? Vulnerabilities { get; set; }

    [JsonPropertyName("published")]
    public NuGetCatalogContextEntry? Published { get; set; }

    [JsonPropertyName("created")]
    public NuGetCatalogContextEntry? Created { get; set; }

    [JsonPropertyName("lastEdited")]
    public NuGetCatalogContextEntry? LastEdited { get; set; }

    [JsonPropertyName("catalog:commitTimeStamp")]
    public NuGetCatalogContextEntry? CatalogCommitTimeStamp { get; set; }

    [JsonPropertyName("reasons")]
    public NuGetCatalogContextEntry? Reasons { get; set; }
}