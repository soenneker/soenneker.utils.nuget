using System;
using Soenneker.Utils.NuGet.Abstract;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Soenneker.NuGet.Client.Abstract;
using Soenneker.Extensions.HttpResponseMessage;
using Soenneker.Utils.NuGet.Responses;
using Soenneker.Extensions.String;
using Soenneker.Utils.NuGet.Responses.Partials;
using Soenneker.Extensions.Enumerable;
using System.Linq;
using System.Collections.Concurrent;

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
        HttpClient client = await _nuGetClient.Get();

        string baseUri = await GetSearchQueryService(source);

        string uri = baseUri + $"?q={packageName.ToLowerInvariantFast()}&prerelease=true&semVerLevel=2.0.0";
        HttpResponseMessage httpResponseMessage = await client.GetAsync(uri, cancellationToken);

        var response = await httpResponseMessage.To<NuGetSearchResponse>();

        return response;
    }

    public async ValueTask<NuGetIndexResponse> GetIndex(string source = "https://api.nuget.org/v3/index.json")
    {
        HttpClient client = await _nuGetClient.Get();

        HttpResponseMessage indexResponse = await client.GetAsync(source);

        var index = await indexResponse.To<NuGetIndexResponse>();

        if (index == null || index.Resources.IsNullOrEmpty())
            throw new InvalidOperationException("Index is not properly formatted or empty");

        return index;
    }

    public async ValueTask<string> GetServiceFromSource(string service, string source = "https://api.nuget.org/v3/index.json")
    {
        NuGetIndexResponse index = await GetIndex(source);

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
    public async ValueTask<string> GetSearchQueryService(string source = "https://api.nuget.org/v3/index.json")
    {
        const string service = "SearchQueryService";

        var key = $"{source}-{service}";

        if (_sourceIndexDict.TryGetValue(source, out string? index))
            return index;

        index = await GetServiceFromSource(service, source);

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<string> GetPackageBaseAddressService(string source = "https://api.nuget.org/v3/index.json")
    {
        const string service = "PackageBaseAddress/3.0.0";

        var key = $"{source}-{service}";

        if (_sourceIndexDict.TryGetValue(source, out string? index))
            return index;

        index = await GetServiceFromSource(service, source);

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<string> GetPackagePublishService(string source = "https://api.nuget.org/v3/index.json")
    {
        const string service = "PackagePublish/2.0.0";

        var key = $"{source}-{service}";

        if (_sourceIndexDict.TryGetValue(key, out string? index))
            return index;

        index = await GetServiceFromSource(service, source);

        _sourceIndexDict.TryAdd(key, index);

        return index;
    }

    public async ValueTask<NuGetPackageVersionsResponse?> GetAllVersions(string packageName, string source = "https://api.nuget.org/v3/index.json")
    {
        HttpClient client = await _nuGetClient.Get();

        string packageBaseAddress = await GetPackageBaseAddressService(source);

        var packageUrl = $"{packageBaseAddress}{packageName.ToLowerInvariantFast()}/index.json";

        HttpResponseMessage packageResponse = await client.GetAsync(packageUrl);

        var response = await packageResponse.To<NuGetPackageVersionsResponse>();

        return response;
    }

    public async ValueTask<List<string>> GetAllListedVersions(string packageName, string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        NuGetSearchResponse searchResult = await Search(packageName, source, cancellationToken);

        var result = new List<string>();

        List<NuGetPackageVersionResponse>? nuGetVersions = searchResult.Data?.FirstOrDefault()?.Versions;

        if (nuGetVersions.IsNullOrEmpty())
            return result;

        result = nuGetVersions.Select(c => c.VersionNumber).ToList()!;

        return result;
    }

    public async ValueTask DeleteAllVersions(string packageName, string apiKey, bool log = true, string source = "https://api.nuget.org/v3/index.json")
    {
        List<string> versions = await GetAllListedVersions(packageName, source);

        foreach (string version in versions)
        {
            await Delete(packageName, version, apiKey, log, source);
        }
    }

    public async ValueTask Delete(string packageName, string version, string apiKey, bool log = true, string source = "https://api.nuget.org/v3/index.json")
    {
        HttpClient client = await _nuGetClient.Get();

        string baseUri = await GetPackagePublishService(source);

        var httpMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{baseUri}/{packageName.ToLowerInvariantFast()}/{version}")
        };

        httpMessage.Headers.Add("X-NuGet-ApiKey", apiKey);

        try
        {
            HttpResponseMessage result = await client.SendAsync(httpMessage);
            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception deleting package ({package}) with version ({version})", packageName, version);
        }
    }
}