using System;

namespace Soenneker.Utils.NuGet.Tests;

internal sealed class DeleteTestClock : TimeProvider
{
    private DateTimeOffset _now = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
    public override DateTimeOffset GetUtcNow() => _now;
    public void Advance(TimeSpan duration) => _now += duration;
}
