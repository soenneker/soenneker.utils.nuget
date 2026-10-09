using System;
using Soenneker.Asyncs.Locks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Utils.NuGet;

internal sealed class NuGetDeleteRateLimiter
{
    // Share quotas across utility instances and package cleanups, without retaining API keys.
    private static readonly ConcurrentDictionary<(string Authority, string KeyHash), NuGetDeleteRateLimiter> _limiters = new();
    private readonly Queue<DateTimeOffset> _requests = new();
    private readonly AsyncLock _lock = new();
    private readonly TimeProvider _timeProvider;
    private DateTimeOffset _pausedUntil;

    internal NuGetDeleteRateLimiter(TimeProvider? timeProvider = null) => _timeProvider = timeProvider ?? TimeProvider.System;

    internal static NuGetDeleteRateLimiter Get(Uri publishUri, string apiKey)
    {
        string keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(apiKey)));
        return _limiters.GetOrAdd((publishUri.GetLeftPart(UriPartial.Authority), keyHash), static _ => new NuGetDeleteRateLimiter());
    }

    internal async ValueTask<TimeSpan> TryAcquire(CancellationToken cancellationToken = default)
    {
        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            DateTimeOffset now = _timeProvider.GetUtcNow();
            while (_requests.TryPeek(out DateTimeOffset oldest) && now - oldest >= TimeSpan.FromHours(1))
                _requests.Dequeue();

            DateTimeOffset next = _pausedUntil;
            if (_requests.Count >= 240 && _requests.Peek().AddHours(1) > next)
                next = _requests.Peek().AddHours(1);

            if (next > now)
                return next - now;

            _requests.Enqueue(now);
            return TimeSpan.Zero;
        }
    }

    internal async ValueTask Wait(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TimeSpan delay = await TryAcquire(cancellationToken).ConfigureAwait(false);
            if (delay == TimeSpan.Zero)
                return;

            // Recheck shared pauses after waking, and support arbitrarily long Retry-After values.
            await Task.Delay(delay > TimeSpan.FromHours(1) ? TimeSpan.FromHours(1) : delay, _timeProvider, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    internal async ValueTask Pause(TimeSpan delay)
    {
        using (await _lock.Lock().ConfigureAwait(false))
        {
            DateTimeOffset until = _timeProvider.GetUtcNow().Add(delay);
            if (until > _pausedUntil)
                _pausedUntil = until;
        }
    }
}
