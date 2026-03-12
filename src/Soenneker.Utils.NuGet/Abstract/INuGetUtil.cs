using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Soenneker.Utils.NuGet.Responses;
using Soenneker.Utils.NuGet.Responses.Partials;
using static Soenneker.Utils.NuGet.NuGetUtil;

namespace Soenneker.Utils.NuGet.Abstract;

/// <summary>
/// A utility library for various NuGet related operations
/// </summary>
public interface INuGetUtil
{
    /// <summary>
    /// Gets the index of available NuGet services from a specified source.
    /// </summary>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The index of available NuGet services.</returns>
    ValueTask<NuGetIndexResponse> GetIndex(string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    ValueTask<string> GetServiceUri(string service, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for a package by name.
    /// </summary>
    /// <param name="packageName">The name of the package to search for.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The search results for the specified package.</returns>
    ValueTask<NuGetSearchResponse?> Search(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all listed versions of a package.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <param name="sortDescending">Guarantee that the latest version is at 0 index.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all listed versions of the specified package.</returns>
    ValueTask<List<string>> GetAllListedVersions(string packageName, bool sortDescending = false, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    ValueTask<string?> GetLatestListedVersion(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlists all versions of a specified package.
    /// </summary>
    /// <param name="packageName">The name of the package to delete.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="log">Indicates whether to log the deletion process.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken"></param>
    ValueTask DeleteAllVersions(string packageName, string apiKey, bool log = true, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific version of a package.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <param name="version">The specific version to delete.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="log">Indicates whether to log the deletion process.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken"></param>
    ValueTask Delete(string packageName, string version, string apiKey, bool log = true, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all versions of a specified package.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>All versions of the specified package, or null if the package does not exist.</returns>
    ValueTask<NuGetPackageVersionsResponse?> GetAllVersions(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    ValueTask<List<KeyValuePair<string, string>>> GetTransitiveDependencies(
        string packageName,
        string version,
        string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default);

    ValueTask<string?> GetCatalogUri(string packageName, string version, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    ValueTask<List<NuGetDataResponse>> GetAllPackages(string owner, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    ValueTask<int> GetTotalDownloads(string owner, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);
}