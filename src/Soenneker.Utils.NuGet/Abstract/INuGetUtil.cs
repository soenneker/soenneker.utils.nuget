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

    /// <summary>
    /// Resolves a named resource URL from a NuGet V3 service index.
    /// </summary>
    /// <param name="service">The NuGet resource type.</param>
    /// <param name="source">The NuGet package-source URL.</param>
    /// <param name="cancellationToken">Signals that the operation should stop.</param>
    /// <returns>The resolved service URL.</returns>
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

    /// <summary>
    /// Finds the latest listed stable version of a package from a NuGet source.
    /// </summary>
    /// <param name="packageName">The NuGet package identifier.</param>
    /// <param name="source">The NuGet package-source URL.</param>
    /// <param name="cancellationToken">Signals that the operation should stop.</param>
    /// <returns>The version, or null when none is listed.</returns>
    ValueTask<string?> GetLatestListedVersion(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlists all versions of a specified package.
    /// </summary>
    /// <param name="packageName">The name of the package to delete.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="log">Indicates whether to log the deletion process.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Unlists all versions of a specified package.</returns>
    ValueTask DeleteAllVersions(string packageName, string apiKey, bool log = true, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends the package source's delete request for a specific version. NuGet.org interprets this as unlisting.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <param name="version">The specific version to delete.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="log">Indicates whether to log the deletion process.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Deletes a specific version of a package.</returns>
    ValueTask Delete(string packageName, string version, string apiKey, bool log = true, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all versions of a specified package.
    /// </summary>
    /// <param name="packageName">The name of the package.</param>
    /// <param name="source">The NuGet API index.json endpoint URL. Defaults to the official NuGet API endpoint.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>All versions of the specified package, or null if the package does not exist.</returns>
    ValueTask<NuGetPackageVersionsResponse?> GetAllVersions(string packageName, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Traverses dependency metadata for a package version using the lower version extracted from supported ranges.
    /// </summary>
    /// <param name="packageName">The NuGet package identifier.</param>
    /// <param name="version">The exact package version.</param>
    /// <param name="source">The NuGet package-source URL.</param>
    /// <param name="cancellationToken">Signals that the operation should stop.</param>
    /// <returns>De-duplicated identifiers and extracted versions encountered across dependency groups.</returns>
    ValueTask<List<KeyValuePair<string, string>>> GetTransitiveDependencies(
        string packageName,
        string version,
        string source = NuGetApiIndexUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the catalog-leaf URI for a specific package version.
    /// </summary>
    /// <param name="packageName">The NuGet package identifier.</param>
    /// <param name="version">The exact package version.</param>
    /// <param name="source">The NuGet package-source URL.</param>
    /// <param name="cancellationToken">Signals that the operation should stop.</param>
    /// <returns>The catalog URI, or null when absent.</returns>
    ValueTask<string?> GetCatalogUri(string packageName, string version, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all NuGet registration entries owned by a profile.
    /// </summary>
    /// <param name="owner">The NuGet profile or owner.</param>
    /// <param name="source">The NuGet package-source URL.</param>
    /// <param name="cancellationToken">Signals that the operation should stop.</param>
    /// <returns>All matching NuGet entries.</returns>
    ValueTask<List<NuGetDataResponse>> GetAllPackages(string owner, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sums download counts for all NuGet packages owned by a profile.
    /// </summary>
    /// <param name="owner">The NuGet profile or owner.</param>
    /// <param name="source">The NuGet package-source URL.</param>
    /// <param name="cancellationToken">Signals that the operation should stop.</param>
    /// <returns>The combined download count.</returns>
    ValueTask<int> GetTotalDownloads(string owner, string source = NuGetApiIndexUri, CancellationToken cancellationToken = default);
}
