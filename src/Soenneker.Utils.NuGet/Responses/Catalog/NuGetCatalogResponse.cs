using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Utils.NuGet.Responses.Catalog.Partials;

namespace Soenneker.Utils.NuGet.Responses.Catalog;

public record NuGetCatalogResponse
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@type")]
    public List<string>? Type { get; set; }

    [JsonPropertyName("authors")]
    public string? Authors { get; set; }

    [JsonPropertyName("catalog:commitId")]
    public string? CatalogCommitId { get; set; }

    [JsonPropertyName("catalog:commitTimeStamp")]
    public DateTime? CatalogCommitTimeStamp { get; set; }

    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("iconFile")]
    public string? IconFile { get; set; }

    [JsonPropertyName("id")]
    public string? PackageId { get; set; }

    [JsonPropertyName("isPrerelease")]
    public bool? IsPrerelease { get; set; }

    [JsonPropertyName("lastEdited")]
    public DateTime? LastEdited { get; set; }

    [JsonPropertyName("licenseExpression")]
    public string? LicenseExpression { get; set; }

    [JsonPropertyName("licenseUrl")]
    public string? LicenseUrl { get; set; }

    [JsonPropertyName("listed")]
    public bool? Listed { get; set; }

    [JsonPropertyName("packageHash")]
    public string? PackageHash { get; set; }

    [JsonPropertyName("packageHashAlgorithm")]
    public string? PackageHashAlgorithm { get; set; }

    [JsonPropertyName("packageSize")]
    public long? PackageSize { get; set; }

    [JsonPropertyName("projectUrl")]
    public string? ProjectUrl { get; set; }

    [JsonPropertyName("published")]
    public DateTime? Published { get; set; }

    [JsonPropertyName("readmeFile")]
    public string? ReadmeFile { get; set; }

    [JsonPropertyName("repository")]
    public string? Repository { get; set; }

    [JsonPropertyName("verbatimVersion")]
    public string? VerbatimVersion { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("dependencyGroups")]
    public List<NuGetDependencyGroup>? DependencyGroups { get; set; }

    [JsonPropertyName("packageEntries")]
    public List<NuGetPackageEntry>? PackageEntries { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("@context")]
    public NuGetContext? Context { get; set; }
}