using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetDataResponse
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    [JsonPropertyName("registration")]
    public string? Registration { get; set; }

    [JsonPropertyName("id")]
    public string? PackageId { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("licenseUrl")]
    public string? LicenseUrl { get; set; }

    [JsonPropertyName("projectUrl")]
    public string? ProjectUrl { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("authors")]
    public List<string>? Authors { get; set; }

    [JsonPropertyName("owners")]
    public List<string>? Owners { get; set; }

    [JsonPropertyName("totalDownloads")]
    public int TotalDownloads { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("packageTypes")]
    public List<NuGetPackageTypeResponse>? PackageTypes { get; set; }

    [JsonPropertyName("versions")]
    public List<NuGetPackageVersionResponse>? Versions { get; set; }

    [JsonPropertyName("deprecation")]
    public NuGetDeprecationResponse? Deprecation { get; set; }

    [JsonPropertyName("vulnerabilities")]
    public List<object>? Vulnerabilities { get; set; } // Assuming no detailed structure provided for vulnerabilities
}