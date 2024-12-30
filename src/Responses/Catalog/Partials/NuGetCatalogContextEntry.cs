using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Soenneker.Utils.NuGet.Responses.Catalog.Partials
{
    public record NuGetCatalogContextEntry
    {
        [JsonPropertyName("@id")]
        public string? Id { get; set; }

        [JsonPropertyName("@container")]
        public string? Container { get; set; }
    }
}
