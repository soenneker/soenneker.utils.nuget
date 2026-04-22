using System.Collections.Generic;
using Soenneker.Utils.NuGet.Abstract;
using Soenneker.Tests.HostedUnit;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Utils.NuGet.Responses;
using System.Linq;
using Soenneker.Utils.NuGet.Responses.Partials;
using Soenneker.Tests.Attributes.Local;

namespace Soenneker.Utils.NuGet.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class NuGetUtilTests : HostedUnitTest
{
    private readonly INuGetUtil _util;

    public NuGetUtilTests(Host host) : base(host)
    {
        _util = Resolve<INuGetUtil>(true);
    }

    [Skip("Manual")]
    public async ValueTask Search_should_not_throw()
    {
        NuGetSearchResponse result = await _util.Search("");
        result.Should().NotBeNull();
    }

    [Test]
    public async ValueTask GetIndex_should_not_throw()
    {
        NuGetIndexResponse result = await _util.GetIndex(cancellationToken: CancellationToken);
        result.Should().NotBeNull();
    }

    [Skip("Manual")]
    public async ValueTask GetAllVersions_should_not_throw()
    {
        NuGetPackageVersionsResponse? result = await _util.GetAllVersions("");
        result.Should().NotBeNull();
    }

    [Skip("Manual")]
    public async ValueTask GetAllListedVersions_should_not_throw()
    {
        List<string> result = await _util.GetAllListedVersions("", true);
        result.Should().NotBeNull();
    }

    [Skip("Manual")]
    public async ValueTask Delete_should_not_throw()
    {
        await _util.Delete("", "", "");
    }

    [Skip("Manual")]
    public async ValueTask DeleteAllVersions_should_not_throw()
    {
        await _util.DeleteAllVersions("", "");
    }

    [LocalOnly]
    public async ValueTask GetTransitivePackages_should_not_throw()
    {
        List<KeyValuePair<string, string>> result = await _util.GetTransitiveDependencies("soenneker.extensions.string", "4.0.665", cancellationToken: CancellationToken);

        result.Should().NotBeNullOrEmpty();
    }

    [LocalOnly]
    public async ValueTask GetTransitivePackages_should_not_return_original()
    {
        List<KeyValuePair<string, string>> result = await _util.GetTransitiveDependencies("soenneker.extensions.string", "3.0.326", cancellationToken: CancellationToken);

        List<string> keys = result.Select(c => c.Key).ToList();

        string? key = keys.FirstOrDefault(c => c == "Soenneker.Extensions.String");
        key.Should().BeNull();
    }

    [LocalOnly]
    public async ValueTask GetAllPackages()
    {
        List<NuGetDataResponse> result = await _util.GetAllPackages("", cancellationToken: CancellationToken);
    }

    [LocalOnly]
    public async ValueTask GetTotalDownloads()
    {
       var result = await _util.GetTotalDownloads("soenneker", cancellationToken: CancellationToken);
    }
}
