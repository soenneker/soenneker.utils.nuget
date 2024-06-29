using System.Collections.Generic;
using Soenneker.Utils.NuGet.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using Soenneker.Facts.Local;
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

    [LocalFact]
    public async Task Search_should_not_throw()
    {
        NuGetSearchResponse result = await _util.Search("");
        result.Should().NotBeNull();
    }

    [LocalFact]
    public async Task GetAllVersions_should_not_throw()
    {
        NuGetPackageVersionsResponse? result = await _util.GetAllVersions("");
        result.Should().NotBeNull();
    }

    [LocalFact]
    public async Task GetAllListedVersions_should_not_throw()
    {
        List<string> result = await _util.GetAllListedVersions("", true);
        result.Should().NotBeNull();
    }

    [LocalFact]
    public async Task Delete_should_not_throw()
    {
        await _util.Delete("", "", "");
    }

    [LocalFact]
    public async Task DeleteAllVersions_should_not_throw()
    {
        await _util.DeleteAllVersions("", "");
    }
}
