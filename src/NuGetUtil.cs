using System;
using Soenneker.Utils.NuGet.Abstract;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Soenneker.NuGet.Client.Abstract;
using Soenneker.Utils.NuGet.Responses;
using Soenneker.Extensions.String;
using Soenneker.Utils.NuGet.Responses.Partials;
using Soenneker.Extensions.Enumerable;
using System.Linq;
using System.Collections.Concurrent;
using Soenneker.Extensions.HttpClient;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Utils.NuGet.Responses.Catalog;
using Soenneker.Utils.NuGet.Responses.Catalog.Partials;
using System.Text.RegularExpressions;


namespace Soenneker.Utils.NuGet;

/// <inheritdoc cref="INuGetUtil"/>
public class NuGetUtil : INuGetUtil
{
    private readonly ILogger<NuGetUtil> _logger;
    private readonly INuGetClient _nuGetClient;

    private readonly ConcurrentDictionary<string, string> _sourceIndexDict = new();

    private const string _searchQueryService = "SearchQueryService";
    private const string _packageBaseAddressService = "PackageBaseAddress/3.0.0";
    private const string _packagePublishService = "PackagePublish/2.0.0";
    private const string _registrationService = "RegistrationsBaseUrl";

    public const string NuGetApiIndexUri = "https://api.nuget.org/v3/index.json";

    public NuGetUtil(ILogger<NuGetUtil> logger, INuGetClient nuGetClient)
    {
        _logger = logger;
        _nuGetClient = nuGetClient;
    }

    public async ValueTask<NuGetSearchResponse?> Search(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get(cancellationToken).NoSync();

        string baseUri = await GetServiceUri(_searchQueryService, source, cancellationToken).NoSync();

        string uri = baseUri + $"?q={packageName.ToLowerInvariantFast()}&prerelease=true&semVerLevel=2.0.0";

        NuGetSearchResponse? response = await client.TrySendToType<NuGetSearchResponse>(uri, _logger, cancellationToken).NoSync();
        return response;
    }

    public async ValueTask<NuGetIndexResponse> GetIndex(string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get(cancellationToken).NoSync();

        NuGetIndexResponse? response = await client.TrySendToType<NuGetIndexResponse>(source, _logger, cancellationToken).NoSync();

        if (response == null || response.Resources.IsNullOrEmpty())
            throw new InvalidOperationException("Index is not properly formatted or empty");

        return response;
    }

    public async ValueTask<string> GetServiceFromSource(string service, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        NuGetIndexResponse index = await GetIndex(source, cancellationToken).NoSync();

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

        index = await GetServiceFromSource(service, source, cancellationToken).NoSync();

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<string?> GetCatalogUri(string packageName, string version, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        string registrationUri = await GetServiceUri(_registrationService, source, cancellationToken).NoSync();

        HttpClient client = await _nuGetClient.Get(cancellationToken).NoSync();

        var packageRegistrationUri = $"{registrationUri}{packageName.ToLowerInvariantFast()}/{version.ToLowerInvariantFast()}.json";

        NuGetRegistrationResponse? registrationResponse = await client.TrySendToType<NuGetRegistrationResponse>(packageRegistrationUri, _logger, cancellationToken).NoSync();

        return registrationResponse?.CatalogEntry;
    }

    public async ValueTask<NuGetPackageVersionsResponse?> GetAllVersions(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all versions of package ({package})...", packageName);

        HttpClient client = await _nuGetClient.Get(cancellationToken).NoSync();

        string packageBaseAddress = await GetServiceUri(_packageBaseAddressService, source, cancellationToken).NoSync();

        var packageUrl = $"{packageBaseAddress}{packageName.ToLowerInvariantFast()}/index.json";

        NuGetPackageVersionsResponse? response = await client.TrySendToType<NuGetPackageVersionsResponse>(packageUrl, _logger, cancellationToken).NoSync();

        return response;
    }

    public async ValueTask<List<string>> GetAllListedVersions(string packageName, bool sortByDescending = false, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all LISTED versions of package ({package})...", packageName);

        NuGetSearchResponse? searchResult = await Search(packageName, source, cancellationToken).NoSync();

        var result = new List<string>();

        List<NuGetPackageVersionResponse>? nuGetVersions = searchResult?.Data?.FirstOrDefault()?.Versions;

        if (nuGetVersions.IsNullOrEmpty())
            return result;

        result = nuGetVersions.Select(c => c.VersionNumber).ToList()!;

        if (sortByDescending)
            result = OrderVersions(result);

        return result;
    }

    public async ValueTask<string?> GetLatestListedVersion(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        List<string> result = await GetAllListedVersions(packageName, true, source, cancellationToken).NoSync();
        return result.FirstOrDefault();
    }

    public async ValueTask DeleteAllVersions(string packageName, string apiKey, bool log = true, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting all versions of package ({package})...", packageName);

        List<string> versions = await GetAllListedVersions(packageName, false, source, cancellationToken).NoSync();

        foreach (string version in versions)
        {
            await Delete(packageName, version, apiKey, log, source, cancellationToken).NoSync();
        }
    }

    public async ValueTask Delete(string packageName, string version, string apiKey, bool log = true, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get(cancellationToken).NoSync();

        string baseUri = await GetServiceUri(_packagePublishService, source, cancellationToken).NoSync();

        using var httpMessage = new HttpRequestMessage();

        httpMessage.Method = HttpMethod.Delete;
        httpMessage.RequestUri = new Uri($"{baseUri}/{packageName.ToLowerInvariantFast()}/{version}");

        httpMessage.Headers.Add("X-NuGet-ApiKey", apiKey);

        _logger.LogInformation("Deleting package ({package}) with version ({version})...", packageName, version);

        try
        {
            HttpResponseMessage result = await client.SendAsync(httpMessage, cancellationToken).NoSync();
            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception deleting package ({package}) with version ({version})", packageName, version);
        }
    }

    public async ValueTask<List<KeyValuePair<string, string>>> GetTransitiveDependencies(
        string packageName,
        string version,
        string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default)
    {
        packageName = packageName.ToLowerInvariantFast();
        version = version.ToLowerInvariantFast();

        var visited = new HashSet<string>();
        var dependencies = new List<KeyValuePair<string, string>>();
        var toProcess = new Queue<(string Id, string Version)>();
        toProcess.Enqueue((packageName, version));

        HttpClient httpClient = await _nuGetClient.Get(CancellationToken.None).NoSync();

        while (toProcess.Count > 0)
        {
            (string currentId, string currentVersion) = toProcess.Dequeue();

            // Only skip adding the original package to the result
            bool isOriginalPackage = currentId == packageName && currentVersion == version;

            if (!isOriginalPackage && !visited.Add($"{currentId}@{currentVersion}"))
                continue;

            if (!isOriginalPackage)
            {
                // Add the dependency to the result list
                dependencies.Add(new KeyValuePair<string, string>(currentId, currentVersion));
            }

            string? catalogUri = await GetCatalogUri(currentId, currentVersion, source, cancellationToken).NoSync();

            if (catalogUri == null)
            {
                continue;
            }

            NuGetCatalogResponse? packageMetadata = await httpClient.TrySendToType<NuGetCatalogResponse>(catalogUri, _logger, cancellationToken).NoSync();

            if (packageMetadata?.DependencyGroups == null)
                continue;

            foreach (NuGetDependencyGroup group in packageMetadata.DependencyGroups)
            {
                if (group.Dependencies == null)
                    continue;

                foreach (NuGetDependency dependency in group.Dependencies)
                {
                    toProcess.Enqueue((dependency.DependencyId, ExtractVersionFromRange(dependency.Range)));
                }
            }
        }

        return dependencies;
    }


    private static string ExtractVersionFromRange(string range)
    {
        // Use a regex to match a version number at the start of the string
        Match match = Regex.Match(range, @"\[(\d+\.\d+\.\d+)");
        return match.Success ? match.Groups[1].Value : "";
    }

    private static List<string> OrderVersions(List<string> input)
    {
        IEnumerable<Version> versions = input.Select(c => new Version(c));
        IOrderedEnumerable<Version> ordered = versions.OrderByDescending(v => v);
        return ordered.Select(c => c.ToString()).ToList();
    }
}