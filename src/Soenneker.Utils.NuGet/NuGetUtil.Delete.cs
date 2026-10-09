using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.String;

namespace Soenneker.Utils.NuGet;

public sealed partial class NuGetUtil
{
    public ValueTask DeleteAllVersions(string packageName, string apiKey, bool log = true, string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default) => DeleteAllVersions(packageName, apiKey, 4, log, source, cancellationToken);

    public async ValueTask DeleteAllVersions(string packageName, string apiKey, int maxConcurrency, bool log = true,
        string source = NuGetApiIndexUri, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxConcurrency, 1);
        List<string> versions = await GetAllListedVersions(packageName, false, source, cancellationToken).ConfigureAwait(false);
        if (log)
            _logger.LogInformation("Found {Count} listed versions of package ({Package}) to delete with concurrency {Concurrency}.",
                versions.Count, packageName, maxConcurrency);

        if (versions.Count == 0)
            return;

        // Resolve the publish service once before starting workers.
        await GetServiceUri(_packagePublishService, source, cancellationToken).ConfigureAwait(false);
        var completed = 0;
        var options = new ParallelOptions { MaxDegreeOfParallelism = maxConcurrency, CancellationToken = cancellationToken };
        await Parallel.ForEachAsync(versions, options, async (version, token) =>
        {
            await Delete(packageName, version, apiKey, log, source, token).ConfigureAwait(false);
            int progress = Interlocked.Increment(ref completed);
            if (log)
                _logger.LogInformation("Deleted {Completed}/{Total} versions of package ({Package}).", progress, versions.Count, packageName);
        }).ConfigureAwait(false);

        if (log)
            _logger.LogInformation("Finished deleting all listed versions of package ({Package}).", packageName);
    }

    public async ValueTask Delete(string packageName, string version, string apiKey, bool log = true, string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default)
    {
        HttpClient client = await _nuGetClient.Get(cancellationToken).ConfigureAwait(false);
        string baseUri = await GetServiceUri(_packagePublishService, source, cancellationToken).ConfigureAwait(false);
        var publishUri = new Uri(baseUri);
        NuGetDeleteRateLimiter limiter = NuGetDeleteRateLimiter.Get(publishUri, apiKey);
        var requestUri = new Uri($"{baseUri.TrimEnd('/')}/{packageName.ToLowerInvariantFast()}/{version}");

        try
        {
            // Bound retries so an invalid or permanently throttled request eventually reaches the caller.
            for (var attempt = 0; ; attempt++)
            {
                long waitStarted = Stopwatch.GetTimestamp();
                await limiter.Wait(cancellationToken).ConfigureAwait(false);
                TimeSpan waited = Stopwatch.GetElapsedTime(waitStarted);
                using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
                request.Headers.Add("X-NuGet-ApiKey", apiKey);
                long started = Stopwatch.GetTimestamp();
                using HttpResponseMessage response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (log)
                    _logger.LogInformation(
                        "Delete ({Package}) version ({Version}), attempt {Attempt}: HTTP {StatusCode} in {RequestSeconds:F2}s; rate-limit wait {WaitSeconds:F2}s.",
                        packageName, version, attempt + 1, (int)response.StatusCode, Stopwatch.GetElapsedTime(started).TotalSeconds, waited.TotalSeconds);

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    TimeSpan delay = response.Headers.RetryAfter?.Delta
                        ?? (response.Headers.RetryAfter?.Date - DateTimeOffset.UtcNow)
                        ?? TimeSpan.FromMinutes(1);
                    if (delay < TimeSpan.FromSeconds(1))
                        delay = TimeSpan.FromSeconds(1);
                    limiter.Pause(delay);
                    if (log)
                        _logger.LogWarning("NuGet throttled deletion of ({Package}) version ({Version}); pausing this API key for {Seconds:F2}s.",
                            packageName, version, delay.TotalSeconds);
                    if (attempt < 3)
                        continue;
                }

                response.EnsureSuccessStatusCode();
                return;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (log)
                _logger.LogError(ex, "Exception deleting package ({Package}) with version ({Version})", packageName, version);
            throw;
        }
    }
}
