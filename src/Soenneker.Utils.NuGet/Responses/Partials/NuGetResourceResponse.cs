using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

/// <summary>
/// Represents the nu get resource response record.
/// </summary>
public record NuGetResourceResponse
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
    /// Gets or sets comment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}