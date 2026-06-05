using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Utils.NuGet.Responses.Partials;

namespace Soenneker.Utils.NuGet.Responses;

/// <summary>
/// Represents the nu get registration response record.
/// </summary>
public record NuGetRegistrationResponse
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
    /// Gets or sets catalog entry.
    /// </summary>
    [JsonPropertyName("catalogEntry")]
    public string? CatalogEntry { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether listed.
    /// </summary>
    [JsonPropertyName("listed")]
    public bool? Listed { get; set; }

    /// <summary>
    /// Gets or sets package content.
    /// </summary>
    [JsonPropertyName("packageContent")]
    public string? PackageContent { get; set; }

    /// <summary>
    /// Gets or sets published.
    /// </summary>
    [JsonPropertyName("published")]
    public DateTime? Published { get; set; }

    /// <summary>
    /// Gets or sets registration.
    /// </summary>
    [JsonPropertyName("registration")]
    public string? Registration { get; set; }

    /// <summary>
    /// Gets or sets context.
    /// </summary>
    [JsonPropertyName("@context")]
    public NuGetContextResponse? Context { get; set; }
}