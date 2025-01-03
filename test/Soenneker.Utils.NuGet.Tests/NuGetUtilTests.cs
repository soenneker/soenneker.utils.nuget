using System.Collections.Generic;
using Soenneker.Utils.NuGet.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using Soenneker.Facts.Manual;
using Soenneker.Utils.NuGet.Responses;
using System.Linq;
using Soenneker.Utils.NuGet.Responses.Partials;
using Soenneker.Facts.Local;

namespace Soenneker.Utils.NuGet.Tests;

[Collection("Collection")]
public class NuGetUtilTests : FixturedUnitTest
{
    private readonly INuGetUtil _util;

    public NuGetUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<INuGetUtil>(true);
    }

    [ManualFact]
    public async ValueTask Search_should_not_throw()
    {
        NuGetSearchResponse result = await _util.Search("");
        result.Should().NotBeNull();
    }

    [Fact]
    public async ValueTask GetIndex_should_not_throw()
    {
        NuGetIndexResponse result = await _util.GetIndex(cancellationToken: CancellationToken);
        result.Should().NotBeNull();
    }

    [ManualFact]
    public async ValueTask GetAllVersions_should_not_throw()
    {
        NuGetPackageVersionsResponse? result = await _util.GetAllVersions("");
        result.Should().NotBeNull();
    }

    [ManualFact]
    public async ValueTask GetAllListedVersions_should_not_throw()
    {
        List<string> result = await _util.GetAllListedVersions("", true);
        result.Should().NotBeNull();
    }

    [ManualFact]
    public async ValueTask Delete_should_not_throw()
    {
        await _util.Delete("", "", "");
    }

    [ManualFact]
    public async ValueTask DeleteAllVersions_should_not_throw()
    {
        await _util.DeleteAllVersions("", "");
    }

    [Fact]
    public async ValueTask GetTransitivePackages_should_not_throw()
    {
        List<KeyValuePair<string, string>> result = await _util.GetTransitiveDependencies("soenneker.extensions.string", "3.0.326", cancellationToken: CancellationToken);

        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async ValueTask GetTransitivePackages_should_not_return_original()
    {
        List<KeyValuePair<string, string>> result = await _util.GetTransitiveDependencies("soenneker.extensions.string", "3.0.326", cancellationToken: CancellationToken);

        List<string> keys = result.Select(c => c.Key).ToList();

        string? key = keys.FirstOrDefault(c => c == "Soenneker.Extensions.String");
        key.Should().BeNull();
    }

    [LocalFact]
    public async ValueTask GetAllPackages()
    {
        List<NuGetDataResponse> result = await _util.GetAllPackages("", cancellationToken: CancellationToken);
    }

    [LocalFact]
    public async ValueTask GetTotalDownloads()
    {
       var result = await _util.GetTotalDownloads("", cancellationToken: CancellationToken);
    }
}