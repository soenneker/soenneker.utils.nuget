using System.Collections.Generic;
using Soenneker.Utils.NuGet.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using Soenneker.Facts.Manual;
using Soenneker.Utils.NuGet.Responses;

namespace Soenneker.Utils.NuGet.Tests;

[Collection("Collection")]
public class NuGetUtilTests : FixturedUnitTest
{
    private readonly INuGetUtil _util;

    public NuGetUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<INuGetUtil>(true);
    }

    [Fact]
    public void Default() { }

    [ManualFact]
    public async Task Search_should_not_throw()
    {
        NuGetSearchResponse result = await _util.Search("");
        result.Should().NotBeNull();
    }

    [ManualFact]
    public async Task GetAllVersions_should_not_throw()
    {
        NuGetPackageVersionsResponse? result = await _util.GetAllVersions("");
        result.Should().NotBeNull();
    }

    [ManualFact]
    public async Task GetAllListedVersions_should_not_throw()
    {
        List<string> result = await _util.GetAllListedVersions("", true);
        result.Should().NotBeNull();
    }

    [ManualFact]
    public async Task Delete_should_not_throw()
    {
        await _util.Delete("", "", "");
    }

    [ManualFact]
    public async Task DeleteAllVersions_should_not_throw()
    {
        await _util.DeleteAllVersions("", "");
    }
}
