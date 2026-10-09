using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Soenneker.Utils.NuGet.Tests;

public sealed class NuGetDeleteTests
{
    private const string Source = "https://delete.invalid/v3/index.json";

    [Test]
    public async Task Quota_expires_per_request_instead_of_sleeping_after_a_batch()
    {
        var clock = new DeleteTestClock();
        var limiter = new NuGetDeleteRateLimiter(clock);
        for (var i = 0; i < 120; i++)
            (await limiter.TryAcquire()).Should().Be(TimeSpan.Zero);
        clock.Advance(TimeSpan.FromMinutes(30));
        for (var i = 0; i < 120; i++)
            (await limiter.TryAcquire()).Should().Be(TimeSpan.Zero);
        (await limiter.TryAcquire()).Should().Be(TimeSpan.FromMinutes(30));
        clock.Advance(TimeSpan.FromMinutes(30));
        for (var i = 0; i < 120; i++)
            (await limiter.TryAcquire()).Should().Be(TimeSpan.Zero);
        (await limiter.TryAcquire()).Should().Be(TimeSpan.FromMinutes(30));
    }

    [Test]
    public async Task Shared_pause_can_only_be_extended_and_keys_are_isolated()
    {
        var clock = new DeleteTestClock();
        var limiter = new NuGetDeleteRateLimiter(clock);
        await limiter.Pause(TimeSpan.FromMinutes(2));
        await limiter.Pause(TimeSpan.FromMinutes(1));
        (await limiter.TryAcquire()).Should().Be(TimeSpan.FromMinutes(2));
        clock.Advance(TimeSpan.FromMinutes(2));
        (await limiter.TryAcquire()).Should().Be(TimeSpan.Zero);

        string key = Guid.NewGuid().ToString();
        var uri = new Uri(Source);
        NuGetDeleteRateLimiter.Get(uri, key).Should().BeSameAs(NuGetDeleteRateLimiter.Get(uri, key));
        NuGetDeleteRateLimiter.Get(uri, key).Should().NotBeSameAs(NuGetDeleteRateLimiter.Get(uri, key + "other"));
    }

    [Test]
    public async Task Rate_limit_wait_is_cancellable(CancellationToken cancellationToken)
    {
        var limiter = new NuGetDeleteRateLimiter();
        await limiter.Pause(TimeSpan.FromHours(1));
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task wait = limiter.Wait(stop.Token).AsTask();
        wait.IsCompleted.Should().BeFalse();
        stop.Cancel();
        await ((Func<Task>)(() => wait)).Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    [Arguments(4)]
    [Arguments(2)]
    public async Task Bulk_deletes_use_bounded_concurrency(int concurrency, CancellationToken cancellationToken)
    {
        var reachedLimit = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var paths = new ConcurrentBag<string>();
        var active = 0;
        var exceeded = 0;
        using var client = new DeleteTestClient(new DeleteTestHandler(async (request, token) =>
        {
            int current = Interlocked.Increment(ref active);
            if (current > concurrency) Interlocked.Exchange(ref exceeded, 1);
            if (current == concurrency) reachedLimit.TrySetResult();
            try
            {
                await release.Task.WaitAsync(token);
                paths.Add(request.RequestUri!.AbsolutePath);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            finally { Interlocked.Decrement(ref active); }
        }));
        var util = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        string key = Guid.NewGuid().ToString();
        Task deletion = concurrency == 4
            ? util.DeleteAllVersions("Test.Package", key, source: Source, cancellationToken: cancellationToken).AsTask()
            : util.DeleteAllVersions("Test.Package", key, concurrency, source: Source, cancellationToken: cancellationToken).AsTask();
        try
        {
            await reachedLimit.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
            deletion.IsCompleted.Should().BeFalse();
        }
        finally { release.TrySetResult(); }
        await deletion;
        exceeded.Should().Be(0);
        paths.Should().HaveCount(6).And.OnlyHaveUniqueItems();
        paths.Should().OnlyContain(path => path.StartsWith("/api/v2/package/test.package/"));
    }

    [Test]
    public async Task Throttle_retries_with_a_fresh_request(CancellationToken cancellationToken)
    {
        var attempts = 0;
        var requests = new ConcurrentBag<HttpRequestMessage>();
        using var client = new DeleteTestClient(new DeleteTestHandler((request, _) =>
        {
            requests.Add(request);
            var response = new HttpResponseMessage(Interlocked.Increment(ref attempts) == 1
                ? HttpStatusCode.TooManyRequests : HttpStatusCode.NoContent);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));
            return Task.FromResult(response);
        }));
        var util = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        await util.Delete("Test.Package", "1.0.0", Guid.NewGuid().ToString(), source: Source, cancellationToken: cancellationToken);
        attempts.Should().Be(2);
        requests.Should().OnlyHaveUniqueItems();
    }

    [Test]
    public async Task Throttle_pauses_other_utility_instances_and_can_be_cancelled(CancellationToken cancellationToken)
    {
        var attempts = 0;
        using var firstStop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        using var client = new DeleteTestClient(new DeleteTestHandler((_, _) =>
        {
            Interlocked.Increment(ref attempts);
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddHours(1));
            return Task.FromResult(response);
        }));
        string key = Guid.NewGuid().ToString();
        var first = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        Task firstPending = first.Delete("Test.Package", "1.0.0", key, source: Source, cancellationToken: firstStop.Token).AsTask();
        firstPending.IsCompleted.Should().BeFalse();
        firstStop.Cancel();
        await ((Func<Task>)(() => firstPending)).Should().ThrowAsync<OperationCanceledException>();
        var second = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        using var secondStop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task pending = second.Delete("Test.Package", "2.0.0", key, source: Source, cancellationToken: secondStop.Token).AsTask();
        pending.IsCompleted.Should().BeFalse();
        secondStop.Cancel();
        await ((Func<Task>)(() => pending)).Should().ThrowAsync<OperationCanceledException>();
        attempts.Should().Be(1);
    }

    [Test]
    public async Task Concurrent_callers_cannot_exceed_the_shared_quota()
    {
        var limiter = new NuGetDeleteRateLimiter();
        var admitted = 0;
        await Parallel.ForAsync(0, 1000, async (_, token) =>
        {
            if (await limiter.TryAcquire(token) == TimeSpan.Zero)
                Interlocked.Increment(ref admitted);
        });
        admitted.Should().Be(240);
    }

    [Test]
    public async Task Persistent_throttling_stops_after_three_retries(CancellationToken cancellationToken)
    {
        var attempts = 0;
        using var client = new DeleteTestClient(new DeleteTestHandler((_, _) =>
        {
            Interlocked.Increment(ref attempts);
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));
            return Task.FromResult(response);
        }));
        var util = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        await ((Func<Task>)(() => util.Delete("Test.Package", "1.0.0", Guid.NewGuid().ToString(), source: Source,
            cancellationToken: cancellationToken).AsTask())).Should().ThrowAsync<HttpRequestException>();
        attempts.Should().Be(4);
    }

    [Test]
    public async Task Bulk_failure_does_not_report_success_or_start_remaining_versions(CancellationToken cancellationToken)
    {
        var attempts = 0;
        using var client = new DeleteTestClient(new DeleteTestHandler((_, _) =>
        {
            Interlocked.Increment(ref attempts);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden));
        }));
        var util = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        await ((Func<Task>)(() => util.DeleteAllVersions("Test.Package", Guid.NewGuid().ToString(), 1, source: Source,
            cancellationToken: cancellationToken).AsTask())).Should().ThrowAsync<HttpRequestException>();
        attempts.Should().Be(1);
    }

    [Test]
    public async Task Http_failures_propagate_without_retry(CancellationToken cancellationToken)
    {
        var attempts = 0;
        using var client = new DeleteTestClient(new DeleteTestHandler((_, _) =>
        {
            Interlocked.Increment(ref attempts);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden));
        }));
        var util = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        await ((Func<Task>)(() => util.Delete("Test.Package", "1.0.0", Guid.NewGuid().ToString(), source: Source,
            cancellationToken: cancellationToken).AsTask())).Should().ThrowAsync<HttpRequestException>();
        attempts.Should().Be(1);
    }
}
