using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Utils.NuGet.Responses;

public record NuGetPackageVersionsResponse
{
    [JsonPropertyName("versions")]
    public List<string>? Versions { get; set; }
}