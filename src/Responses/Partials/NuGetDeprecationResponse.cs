using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses.Partials;

public record NuGetDeprecationResponse
{
    [JsonPropertyName("alternatePackage")]
    public NuGetAlternatePackageResponse? AlternatePackage { get; set; }

    [JsonPropertyName("reasons")]
    public List<string>? Reasons { get; set; }
}