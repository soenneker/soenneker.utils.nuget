using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Utils.NuGet.Responses.Catalog.Partials;

namespace Soenneker.Utils.NuGet.Responses.Catalog;

/// <summary>
/// Represents the nu get catalog response record.
/// </summary>
public record NuGetCatalogResponse
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
    public List<string>? Type { get; set; }

    /// <summary>
    /// Gets or sets authors.
    /// </summary>
    [JsonPropertyName("authors")]
    public string? Authors { get; set; }

    /// <summary>
    /// Gets or sets catalog commit id.
    /// </summary>
    [JsonPropertyName("catalog:commitId")]
    public string? CatalogCommitId { get; set; }

    /// <summary>
    /// Gets or sets catalog commit time stamp.
    /// </summary>
    [JsonPropertyName("catalog:commitTimeStamp")]
    public DateTime? CatalogCommitTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets copyright.
    /// </summary>
    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }

    /// <summary>
    /// Gets or sets created.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    /// <summary>
    /// Gets or sets description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets icon file.
    /// </summary>
    [JsonPropertyName("iconFile")]
    public string? IconFile { get; set; }

    /// <summary>
    /// Gets or sets package id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? PackageId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the instance is prerelease.
    /// </summary>
    [JsonPropertyName("isPrerelease")]
    public bool? IsPrerelease { get; set; }

    /// <summary>
    /// Gets or sets last edited.
    /// </summary>
    [JsonPropertyName("lastEdited")]
    public DateTime? LastEdited { get; set; }

    /// <summary>
    /// Gets or sets license expression.
    /// </summary>
    [JsonPropertyName("licenseExpression")]
    public string? LicenseExpression { get; set; }

    /// <summary>
    /// Gets or sets license url.
    /// </summary>
    [JsonPropertyName("licenseUrl")]
    public string? LicenseUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether listed.
    /// </summary>
    [JsonPropertyName("listed")]
    public bool? Listed { get; set; }

    /// <summary>
    /// Gets or sets package hash.
    /// </summary>
    [JsonPropertyName("packageHash")]
    public string? PackageHash { get; set; }

    /// <summary>
    /// Gets or sets package hash algorithm.
    /// </summary>
    [JsonPropertyName("packageHashAlgorithm")]
    public string? PackageHashAlgorithm { get; set; }

    /// <summary>
    /// Gets or sets package size.
    /// </summary>
    [JsonPropertyName("packageSize")]
    public long? PackageSize { get; set; }

    /// <summary>
    /// Gets or sets project url.
    /// </summary>
    [JsonPropertyName("projectUrl")]
    public string? ProjectUrl { get; set; }

    /// <summary>
    /// Gets or sets published.
    /// </summary>
    [JsonPropertyName("published")]
    public DateTime? Published { get; set; }

    /// <summary>
    /// Gets or sets readme file.
    /// </summary>
    [JsonPropertyName("readmeFile")]
    public string? ReadmeFile { get; set; }

    /// <summary>
    /// Gets or sets release notes.
    /// </summary>
    [JsonPropertyName("releaseNotes")]
    public string? ReleaseNotes { get; set; }

    /// <summary>
    /// Gets or sets repository.
    /// </summary>
    [JsonPropertyName("repository")]
    [JsonConverter(typeof(NuGetRepositoryConverter))]
    public NuGetRepository? Repository { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether require license acceptance.
    /// </summary>
    [JsonPropertyName("requireLicenseAcceptance")]
    public bool? RequireLicenseAcceptance { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether serviceable.
    /// </summary>
    [JsonPropertyName("serviceable")]
    [JsonConverter(typeof(NuGetFlexibleBooleanConverter))]
    public bool? Serviceable { get; set; }

    /// <summary>
    /// Gets or sets verbatim version.
    /// </summary>
    [JsonPropertyName("verbatimVersion")]
    public string? VerbatimVersion { get; set; }

    /// <summary>
    /// Gets or sets version.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets dependency groups.
    /// </summary>
    [JsonPropertyName("dependencyGroups")]
    public List<NuGetDependencyGroup>? DependencyGroups { get; set; }

    /// <summary>
    /// Gets or sets package entries.
    /// </summary>
    [JsonPropertyName("packageEntries")]
    public List<NuGetPackageEntry>? PackageEntries { get; set; }

    /// <summary>
    /// Gets or sets tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets context.
    /// </summary>
    [JsonPropertyName("@context")]
    public NuGetContext? Context { get; set; }
}