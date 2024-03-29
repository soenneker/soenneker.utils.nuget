using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Utils.NuGet.Abstract;

/// <summary>
/// A utility library for various NuGet related operations
/// </summary>
public interface INuGetUtil
{
    ValueTask<IEnumerable<string>> GetAllPackageVersionsAsync(string packageName, string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default);
}
