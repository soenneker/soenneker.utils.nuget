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


namespace Soenneker.Utils.NuGet;

/// <inheritdoc cref="INuGetUtil"/>
public class NuGetUtil : INuGetUtil
{
    private readonly ILogger<NuGetUtil> _logger;
    private readonly INuGetClient _nuGetClient;

    private readonly ConcurrentDictionary<string, string> _sourceIndexDict = new();

    public NuGetUtil(ILogger<NuGetUtil> logger, INuGetClient nuGetClient)
    {
        _logger = logger;
        _nuGetClient = nuGetClient;
    }

    public async ValueTask<NuGetSearchResponse> Search(string packageName, string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get().NoSync();

        string baseUri = await GetSearchQueryService(source, cancellationToken).NoSync();

        string uri = baseUri + $"?q={packageName.ToLowerInvariantFast()}&prerelease=true&semVerLevel=2.0.0";

        NuGetSearchResponse? response = await client.SendToType<NuGetSearchResponse>(uri, _logger, cancellationToken).NoSync();
        return response!;
    }

    public async ValueTask<NuGetIndexResponse> GetIndex(string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get().NoSync();

        NuGetIndexResponse? response = await client.SendToType<NuGetIndexResponse>(source, _logger, cancellationToken).NoSync();

        if (response == null || response.Resources.IsNullOrEmpty())
            throw new InvalidOperationException("Index is not properly formatted or empty");

        return response;
    }

    public async ValueTask<string> GetServiceFromSource(string service, string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
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

    // TODO: Index util
    // TODO: ConcurrentDictionary async extension
    public async ValueTask<string> GetSearchQueryService(string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        const string service = "SearchQueryService";

        var key = $"{source}-{service}";

        if (_sourceIndexDict.TryGetValue(source, out string? index))
            return index;

        index = await GetServiceFromSource(service, source, cancellationToken).NoSync();

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<string> GetPackageBaseAddressService(string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        const string service = "PackageBaseAddress/3.0.0";

        var key = $"{source}-{service}";

        if (_sourceIndexDict.TryGetValue(source, out string? index))
            return index;

        index = await GetServiceFromSource(service, source, cancellationToken).NoSync();

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<string> GetPackagePublishService(string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        const string service = "PackagePublish/2.0.0";

        var key = $"{source}-{service}";

        if (_sourceIndexDict.TryGetValue(key, out string? index))
            return index;

        index = await GetServiceFromSource(service, source, cancellationToken).NoSync();

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<NuGetPackageVersionsResponse?> GetAllVersions(string packageName, string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get();

        string packageBaseAddress = await GetPackageBaseAddressService(source, cancellationToken).NoSync();

        var packageUrl = $"{packageBaseAddress}{packageName.ToLowerInvariantFast()}/index.json";

        NuGetPackageVersionsResponse? response = await client.SendToType<NuGetPackageVersionsResponse>(packageUrl, _logger, cancellationToken).NoSync();

        return response;
    }

    public async ValueTask<List<string>> GetAllListedVersions(string packageName, bool sortByDescending = false, string source = "https://api.nuget.org/v3/index.json",
        CancellationToken cancellationToken = default)
    {
        NuGetSearchResponse searchResult = await Search(packageName, source, cancellationToken).NoSync();

        var result = new List<string>();

        List<NuGetPackageVersionResponse>? nuGetVersions = searchResult.Data?.FirstOrDefault()?.Versions;

        if (nuGetVersions.IsNullOrEmpty())
            return result;

        result = nuGetVersions.Select(c => c.VersionNumber).ToList()!;

        if (sortByDescending)
            result = OrderVersions(result);

        return result;
    }

    public async ValueTask<string?> GetLatestListedVersion(string packageName, string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        List<string> result = await GetAllListedVersions(packageName, true, source, cancellationToken).NoSync();
        return result.FirstOrDefault();
    }

    public async ValueTask DeleteAllVersions(string packageName, string apiKey, bool log = true, string source = "https://api.nuget.org/v3/index.json")
    {
        List<string> versions = await GetAllListedVersions(packageName, false, source).NoSync();

        foreach (string version in versions)
        {
            await Delete(packageName, version, apiKey, log, source).NoSync();
        }
    }

    public async ValueTask Delete(string packageName, string version, string apiKey, bool log = true, string source = "https://api.nuget.org/v3/index.json")
    {
        HttpClient client = await _nuGetClient.Get().NoSync();

        string baseUri = await GetPackagePublishService(source).NoSync();

        var httpMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{baseUri}/{packageName.ToLowerInvariantFast()}/{version}")
        };

        httpMessage.Headers.Add("X-NuGet-ApiKey", apiKey);

        try
        {
            HttpResponseMessage result = await client.SendAsync(httpMessage).NoSync();
            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception deleting package ({package}) with version ({version})", packageName, version);
        }
    }

    private static List<string> OrderVersions(List<string> input)
    {
        List<Version> versions = input.Select(c => new Version(c)).ToList();
        IOrderedEnumerable<Version> ordered = versions.OrderByDescending(v => v);
        return ordered.Select(c => c.ToString()).ToList();
    }
}