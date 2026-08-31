[![](https://img.shields.io/nuget/v/soenneker.utils.nuget.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.nuget/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.nuget/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.nuget/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.nuget.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.nuget/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.nuget/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.nuget/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.NuGet
Queries NuGet V3 feeds for package metadata and versions, traverses dependency metadata, and submits package unlist requests.

## Installation

```bash
dotnet add package Soenneker.Utils.NuGet
```

## Quick start

```csharp
using Soenneker.Utils.NuGet.Registrars;

services.AddNuGetUtilAsSingleton();
```

Then inject `INuGetUtil` wherever you need it.

Every `source` argument expects a NuGet V3 `index.json` URL, not a package page or flat-container
URL. Omitting it uses nuget.org.

## Search and versions

```csharp
NuGetSearchResponse? search = await nuGetUtil.Search(
    "Soenneker.Utils.Json",
    cancellationToken: cancellationToken);

List<string> versions = await nuGetUtil.GetAllListedVersions(
    "Soenneker.Utils.Json",
    sortDescending: true,
    cancellationToken: cancellationToken);

string? latestStable = await nuGetUtil.GetLatestListedVersion(
    "Soenneker.Utils.Json",
    cancellationToken: cancellationToken);
```

`Search` is a feed search and can return multiple fuzzy matches. Listed-version methods select the
exact package ID from those results. Descending order follows NuGet semantic-version precedence;
`GetLatestListedVersion` skips prerelease versions. An empty list or `null` latest value means no
matching listed version was returned.

`GetAllVersions` uses the feed's package-base-address resource and includes unlisted versions when
the feed exposes them. `GetIndex`, `GetServiceUri`, and `GetCatalogUri` expose lower-level V3
service discovery. Discovered service URLs are cached for the utility's lifetime.

## Unlist a package version

```csharp
await nuGetUtil.Delete(
    packageName: "Contoso.Package",
    version: "1.2.3",
    apiKey: apiKey,
    cancellationToken: cancellationToken);
```

On nuget.org, the V3 delete endpoint unlists a package version; it does not erase the package.
Other feeds can define different delete behavior. `DeleteAllVersions` discovers the listed
versions and submits the same operation for each one in rate-limited batches, so verify the source,
package ID, and credentials before calling it. HTTP failures and requested cancellation propagate.
Setting `log: false` suppresses the per-delete informational and error messages.

## Dependency metadata

```csharp
List<KeyValuePair<string, string>> dependencies =
    await nuGetUtil.GetTransitiveDependencies("Contoso.Package", "1.2.3", cancellationToken: cancellationToken);
```

This is metadata traversal, not NuGet restore resolution. It combines every dependency group,
does not choose a target framework, and extracts only the supported lower-bound form from version
ranges before following it. Do not use the result as a lock file or as proof of the versions NuGet
would install. Results are de-duplicated and cached by source/package/version for the utility's lifetime.

`GetAllPackages(owner)` pages through search results and retains entries whose owners contain the
requested owner. `GetTotalDownloads(owner)` sums the returned version counts into an `int`; feed
support and owner metadata determine how complete those results are.
