using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Soenneker.Utils.NuGet.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using System.Linq;

namespace Soenneker.Utils.NuGet;

/// <inheritdoc cref="INuGetUtil"/>
public class NuGetUtil: INuGetUtil
{
    private readonly ILogger<NuGetUtil> _logger;

    public NuGetUtil(ILogger<NuGetUtil> logger)
    {
        _logger = logger;
    }

    private static async ValueTask<FindPackageByIdResource> BuildFindResource(string source = "https://api.nuget.org/v3/index.json", CancellationToken cancellationToken = default)
    {
        SourceRepository? repository = Repository.Factory.GetCoreV3(source);

        var resource = await repository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);
        
        return resource;
    }

    public async ValueTask<IEnumerable<string>> GetAllPackageVersionsAsync(string packageName, string source = "https://api.nuget.org/v3/index.json",  CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all versions for package ({package})...", packageName);

        FindPackageByIdResource resource = await BuildFindResource(source, cancellationToken);

        var cache = new SourceCacheContext();

        IEnumerable<string> versions = (await resource.GetAllVersionsAsync(
            packageName,
            cache,
            NullLogger.Instance,
            cancellationToken)).Select(c => c.ToString());

        return versions;
    }
}
