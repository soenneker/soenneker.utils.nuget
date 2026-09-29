using System.Text.Json.Serialization;
using Soenneker.Utils.NuGet.Responses;
using Soenneker.Utils.NuGet.Responses.Catalog;
using Soenneker.Utils.NuGet.Responses.Catalog.Partials;

namespace Soenneker.Utils.NuGet;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString)]
[JsonSerializable(typeof(NuGetSearchResponse))]
[JsonSerializable(typeof(NuGetIndexResponse))]
[JsonSerializable(typeof(NuGetRegistrationResponse))]
[JsonSerializable(typeof(NuGetPackageVersionsResponse))]
[JsonSerializable(typeof(NuGetCatalogResponse))]
[JsonSerializable(typeof(NuGetRepository))]
internal partial class LibraryJsonContext : JsonSerializerContext;
