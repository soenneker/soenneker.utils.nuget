using Soenneker.Utils.NuGet.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Utils.NuGet.Tests;

[Collection("Collection")]
public class NuGetUtilTests : FixturedUnitTest
{
    private readonly INuGetUtil _util;

    public NuGetUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<INuGetUtil>(true);
    }
}
