using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Utils.NuGet.Responses.Partials;

namespace Soenneker.Utils.NuGet.Responses;

public record NuGetRegistrationResponse
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@type")]
    public List<string>? Type { get; set; }

    [JsonPropertyName("catalogEntry")]
    public string? CatalogEntry { get; set; }

    [JsonPropertyName("listed")]
    public bool? Listed { get; set; }

    [JsonPropertyName("packageContent")]
    public string? PackageContent { get; set; }

    [JsonPropertyName("published")]
    public DateTime? Published { get; set; }

    [JsonPropertyName("registration")]
    public string? Registration { get; set; }

    [JsonPropertyName("@context")]
    public NuGetContextResponse? Context { get; set; }
}