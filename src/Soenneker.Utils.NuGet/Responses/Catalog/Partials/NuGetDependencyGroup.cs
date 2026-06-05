using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials;

/// <summary>
/// Represents the nu get dependency group record.
/// </summary>
public record NuGetDependencyGroup
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
    /// Gets or sets dependencies.
    /// </summary>
    [JsonPropertyName("dependencies")]
    public List<NuGetDependency>? Dependencies { get; set; }

    /// <summary>
    /// Gets or sets target framework.
    /// </summary>
    [JsonPropertyName("targetFramework")]
    public string? TargetFramework { get; set; }
}