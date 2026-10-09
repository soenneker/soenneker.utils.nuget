using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using Soenneker.Extensions.Enumerable;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.NuGet.Client.Abstract;
using Soenneker.Utils.NuGet.Abstract;
using Soenneker.Utils.NuGet.Responses;
using Soenneker.Utils.NuGet.Responses.Catalog;
using Soenneker.Utils.NuGet.Responses.Catalog.Partials;
using Soenneker.Utils.NuGet.Responses.Partials;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Utils.NuGet;

public sealed partial class NuGetUtil : INuGetUtil
{
    private readonly ILogger<NuGetUtil> _logger;
    private readonly INuGetClient _nuGetClient;

    private readonly ConcurrentDictionary<string, string> _sourceIndexDict = new();

    private const string _searchQueryService = "SearchQueryService";
    private const string _packageBaseAddressService = "PackageBaseAddress/3.0.0";
    private const string _packagePublishService = "PackagePublish/2.0.0";
    private const string _registrationService = "RegistrationsBaseUrl";

    /// <summary>
    /// The nu get api index uri.
    /// </summary>
    public const string NuGetApiIndexUri = "https://api.nuget.org/v3/index.json";

    private readonly ConcurrentDictionary<(string Source, string PackageName, string Version), List<KeyValuePair<string, string>>> _dependencyCache =
        new(SourcePackageVersionKeyComparer.Instance);

    public NuGetUtil(ILogger<NuGetUtil> logger, INuGetClient nuGetClient)
    {
        _logger = logger;
        _nuGetClient = nuGetClient;
    }

    public async ValueTask<NuGetSearchResponse?> Search(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get(cancellationToken)
                                              .NoSync();

        string baseUri = await GetServiceUri(_searchQueryService, source, cancellationToken)
            .NoSync();

        string query = Uri.EscapeDataString(packageName.ToLowerInvariantFast());
        var uri = $"{baseUri}?q={query}&prerelease=true&semVerLevel=2.0.0";

        return await TryGetResponse(client, uri, LibraryJsonContext.Default.NuGetSearchResponse, cancellationToken)
                           .NoSync();
    }

    public async ValueTask<NuGetIndexResponse> GetIndex(string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get(cancellationToken)
                                              .NoSync();

        NuGetIndexResponse? response = await TryGetResponse(client, source, LibraryJsonContext.Default.NuGetIndexResponse, cancellationToken)
                                                   .NoSync();

        if (response == null || response.Resources.IsNullOrEmpty())
            throw new InvalidOperationException("Index is not properly formatted or empty");

        return response;
    }

    /// <summary>
    /// Reads a NuGet service-index resource URL from a package source.
    /// </summary>
    /// <param name="service">The NuGet resource type.</param>
    /// <param name="source">The NuGet package-source URL.</param>
    /// <param name="cancellationToken">Signals that the operation should stop.</param>
    /// <returns>The resolved service URL.</returns>
    public async ValueTask<string> GetServiceFromSource(string service, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        NuGetIndexResponse index = await GetIndex(source, cancellationToken)
            .NoSync();

        foreach (NuGetResourceResponse resource in index.Resources!)
        {
            if (resource.Type != service)
                continue;

            if (resource.Id.IsNullOrEmpty())
                continue;

            return resource.Id;
        }

        throw new InvalidOperationException($"Could not find the service ({service}) from index ({source})");
    }

    public async ValueTask<string> GetServiceUri(string service, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        var key = $"{source}-{service}";

        if (_sourceIndexDict.TryGetValue(key, out string? index))
            return index;

        index = await GetServiceFromSource(service, source, cancellationToken)
            .NoSync();

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<string?> GetCatalogUri(string packageName, string version, string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default)
    {
        string registrationUri = await GetServiceUri(_registrationService, source, cancellationToken)
            .NoSync();

        HttpClient client = await _nuGetClient.Get(cancellationToken)
                                              .NoSync();

        var packageRegistrationUri = $"{registrationUri}{packageName.ToLowerInvariantFast()}/{version.ToLowerInvariantFast()}.json";

        NuGetRegistrationResponse? registrationResponse = await TryGetResponse(client, packageRegistrationUri, LibraryJsonContext.Default.NuGetRegistrationResponse, cancellationToken)
                                                                .NoSync();

        return registrationResponse?.CatalogEntry;
    }

    public async ValueTask<NuGetPackageVersionsResponse?> GetAllVersions(string packageName, string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all versions of package ({package})...", packageName);

        HttpClient client = await _nuGetClient.Get(cancellationToken)
                                              .NoSync();

        string packageBaseAddress = await GetServiceUri(_packageBaseAddressService, source, cancellationToken)
            .NoSync();

        var packageUrl = $"{packageBaseAddress}{packageName.ToLowerInvariantFast()}/index.json";

        return await TryGetResponse(client, packageUrl, LibraryJsonContext.Default.NuGetPackageVersionsResponse, cancellationToken)
                           .NoSync();
    }

    public async ValueTask<List<string>> GetAllListedVersions(string packageName, bool sortByDescending = false, string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all LISTED versions of package ({package})...", packageName);

        NuGetSearchResponse? searchResult = await Search(packageName, source, cancellationToken)
            .NoSync();

        List<NuGetPackageVersionResponse>? nuGetVersions = null;
        List<NuGetDataResponse>? data = searchResult?.Data;

        if (data is not null)
        {
            for (var i = 0; i < data.Count; i++)
            {
                NuGetDataResponse candidate = data[i];
                if (!string.Equals(candidate.PackageId, packageName, StringComparison.OrdinalIgnoreCase))
                    continue;

                nuGetVersions = candidate.Versions;
                break;
            }
        }

        if (nuGetVersions.IsNullOrEmpty())
            return [];

        var result = new List<string>(nuGetVersions.Count);
        for (var i = 0; i < nuGetVersions.Count; i++)
        {
            string? versionNumber = nuGetVersions[i].VersionNumber;
            if (versionNumber.HasContent())
                result.Add(versionNumber);
        }

        if (sortByDescending)
            result = OrderVersions(result);

        return result;
    }

    public async ValueTask<string?> GetLatestListedVersion(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        List<string> versions = await GetAllListedVersions(packageName, true, source, cancellationToken).NoSync();
        for (var i = 0; i < versions.Count; i++)
        {
            string version = versions[i];
            if (!NuGetVersion.Parse(version).IsPrerelease)
                return version;
        }

        return null;
    }

    public async ValueTask<List<KeyValuePair<string, string>>> GetTransitiveDependencies(string packageName, string version, string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default)
    {
        packageName = packageName.ToLowerInvariantFast();
        version = version.ToLowerInvariantFast();

        // Check if the result is already cached
        if (_dependencyCache.TryGetValue((source, packageName, version), out List<KeyValuePair<string, string>>? cachedDependencies))
        {
            return [.. cachedDependencies];
        }

        var visited = new HashSet<(string PackageName, string Version)>(PackageVersionKeyComparer.Instance);
        var queued = new HashSet<(string PackageName, string Version)>(PackageVersionKeyComparer.Instance);
        var seenDependencies = new HashSet<KeyValuePair<string, string>>(DependencyPairComparer.Instance);
        var dependencies = new List<KeyValuePair<string, string>>();
        var toProcess = new Queue<(string Id, string Version)>();
        toProcess.Enqueue((packageName, version));
        queued.Add((packageName, version));

        HttpClient httpClient = await _nuGetClient.Get(cancellationToken)
                                                  .NoSync();

        while (toProcess.Count > 0)
        {
            (string currentId, string currentVersion) = toProcess.Dequeue();

            // Skip if already processed and cached
            if (_dependencyCache.TryGetValue((source, currentId, currentVersion), out List<KeyValuePair<string, string>>? cachedInnerDependencies))
            {
                foreach (KeyValuePair<string, string> cachedDependency in cachedInnerDependencies)
                {
                    if (seenDependencies.Add(cachedDependency))
                        dependencies.Add(cachedDependency);
                }

                continue;
            }

            // Skip if already visited in this traversal
            if (!visited.Add((currentId, currentVersion)))
                continue;

            string? catalogUri = await GetCatalogUri(currentId, currentVersion, source, cancellationToken)
                .NoSync();

            if (catalogUri == null)
            {
                continue;
            }

            NuGetCatalogResponse? packageMetadata = await TryGetResponse(httpClient, catalogUri, LibraryJsonContext.Default.NuGetCatalogResponse, cancellationToken)
                                                                    .NoSync();

            if (packageMetadata?.DependencyGroups == null)
                continue;

            foreach (NuGetDependencyGroup group in packageMetadata.DependencyGroups)
            {
                if (group.Dependencies == null)
                    continue;

                foreach (NuGetDependency dependency in group.Dependencies)
                {
                    if (string.IsNullOrWhiteSpace(dependency.DependencyId) || string.IsNullOrWhiteSpace(dependency.Range))
                        continue;

                    string dependencyVersion = ExtractVersionFromRange(dependency.Range);

                    if (dependencyVersion.IsNullOrEmpty())
                        continue;

                    var dependencyPair = new KeyValuePair<string, string>(dependency.DependencyId, dependencyVersion);

                    if (seenDependencies.Add(dependencyPair))
                        dependencies.Add(dependencyPair);

                    var dependencyKey = (dependency.DependencyId, dependencyVersion);

                    if (!visited.Contains(dependencyKey) && queued.Add(dependencyKey))
                        toProcess.Enqueue(dependencyKey);
                }
            }

        }

        // Cache the final dependencies for the requested package/version
        _dependencyCache[(source, packageName, version)] = [.. dependencies];

        return dependencies;
    }

    public async ValueTask<List<NuGetDataResponse>> GetAllPackages(string owner, string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default)
    {
        var allPackages = new List<NuGetDataResponse>();
        var skip = 0;
        const int take = 100;

        HttpClient client = await _nuGetClient.Get(cancellationToken)
                                              .NoSync();

        string baseUri = await GetServiceUri(_searchQueryService, source, cancellationToken)
            .NoSync();

        while (true)
        {
            string query = Uri.EscapeDataString(owner);
            var searchUrl = $"{baseUri}?q={query}&take={take}&skip={skip}";

            NuGetSearchResponse? altResponse = await TryGetResponse(client, searchUrl, LibraryJsonContext.Default.NuGetSearchResponse, cancellationToken)
                                                           .NoSync();

            if (altResponse == null || !altResponse.Data.Populated())
                break;

            foreach (NuGetDataResponse data in altResponse.Data)
            {
                if (data.Owners.IsNullOrEmpty())
                    continue;

                if (data.Owners.Contains(owner, StringComparer.OrdinalIgnoreCase))
                {
                    allPackages.Add(data);
                }
            }

            if (altResponse.Data.Count < take)
                break; // No more results to paginate through

            skip += take; // Move to the next page
        }

        return allPackages;
    }

    public async ValueTask<int> GetTotalDownloads(string owner, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        // Get all packages for the owner
        List<NuGetDataResponse> allPackages = await GetAllPackages(owner, source, cancellationToken);

        var totalDownloads = 0;

        // Aggregate downloads for all versions of all packages
        foreach (NuGetDataResponse package in allPackages)
        {
            if (package.Versions.Populated())
            {
                foreach (NuGetPackageVersionResponse version in package.Versions)
                {
                    totalDownloads += version.Downloads;
                }
            }
            else
            {
                // If no versions, add the total downloads at the package level
                totalDownloads += package.TotalDownloads;
            }
        }

        return totalDownloads;
    }

    private async ValueTask<T?> TryGetResponse<T>(HttpClient client, string uri, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(uri, cancellationToken).NoSync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request ({uri}) returned a non-successful status code ({statusCode})", uri, response.StatusCode);
                return default;
            }

            return await response.Content.ReadFromJsonAsync(typeInfo, cancellationToken).NoSync();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("HTTP request to {uri} was canceled.", uri);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending HTTP request or reading response.");
            return default;
        }
    }

    private static string ExtractVersionFromRange(string range)
    {
        // Use a regex to match a version number at the start of the string
        Match match = VersionExtractionRegex()
            .Match(range);
        return match.Success ? match.Groups[1].Value : "";
    }

    private static List<string> OrderVersions(List<string> input)
    {
        return input.OrderByDescending(NuGetVersion.Parse)
                    .ToList();
    }

    [GeneratedRegex(@"\[(\d+\.\d+\.\d+)")]
    private static partial Regex VersionExtractionRegex();
}
