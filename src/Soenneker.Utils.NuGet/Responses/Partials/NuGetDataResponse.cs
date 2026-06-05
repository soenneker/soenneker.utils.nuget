using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get data response record.
/// </summary>
public record NuGetDataResponse
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
    /// Gets or sets registration.
    /// </summary>
    [JsonPropertyName("registration")]
    public string? Registration { get; set; }

    /// <summary>
    /// Gets or sets package id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? PackageId { get; set; }

    /// <summary>
    /// Gets or sets version.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets summary.
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets icon url.
    /// </summary>
    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; set; }

    /// <summary>
    /// Gets or sets license url.
    /// </summary>
    [JsonPropertyName("licenseUrl")]
    public string? LicenseUrl { get; set; }

    /// <summary>
    /// Gets or sets project url.
    /// </summary>
    [JsonPropertyName("projectUrl")]
    public string? ProjectUrl { get; set; }

    /// <summary>
    /// Gets or sets tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets authors.
    /// </summary>
    [JsonPropertyName("authors")]
    public List<string>? Authors { get; set; }

    /// <summary>
    /// Gets or sets owners.
    /// </summary>
    [JsonPropertyName("owners")]
    public List<string>? Owners { get; set; }

    /// <summary>
    /// Gets or sets total downloads.
    /// </summary>
    [JsonPropertyName("totalDownloads")]
    public int TotalDownloads { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether verified.
    /// </summary>
    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    /// <summary>
    /// Gets or sets package types.
    /// </summary>
    [JsonPropertyName("packageTypes")]
    public List<NuGetPackageTypeResponse>? PackageTypes { get; set; }

    /// <summary>
    /// Gets or sets versions.
    /// </summary>
    [JsonPropertyName("versions")]
    public List<NuGetPackageVersionResponse>? Versions { get; set; }

    /// <summary>
    /// Gets or sets deprecation.
    /// </summary>
    [JsonPropertyName("deprecation")]
    public NuGetDeprecationResponse? Deprecation { get; set; }

    /// <summary>
    /// Gets or sets vulnerabilities.
    /// </summary>
    [JsonPropertyName("vulnerabilities")]
    public List<object>? Vulnerabilities { get; set; } // Assuming no detailed structure provided for vulnerabilities
}