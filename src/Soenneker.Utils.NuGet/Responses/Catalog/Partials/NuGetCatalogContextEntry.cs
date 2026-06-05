using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials
{
    /// <summary>
    /// Represents the nu get catalog context entry record.
    /// </summary>
    public record NuGetCatalogContextEntry
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [JsonPropertyName("@id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets container.
        /// </summary>
        [JsonPropertyName("@container")]
        public string? Container { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        [JsonPropertyName("@type")]
        public string? Type { get; set; }
    }
}
