[![](https://img.shields.io/nuget/v/soenneker.utils.nuget.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.nuget/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.nuget/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.nuget/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.nuget.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.nuget/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.nuget/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.nuget/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.NuGet
A utility library for various NuGet related operations.

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

## Common operations

- `Search()` - Searches for a package by name. Returns the search results for the specified package.
- `GetIndex()` - Gets the index of available NuGet services from a specified source. Returns the index of available NuGet services.
- `GetServiceFromSource()` - Reads a NuGet service index and returns the resource URL whose `@type` matches the requested service.
- `GetServiceUri()` - Returns the requested NuGet service URL and caches it per source/service pair.
- `GetCatalogUri()` - Returns a package version's catalog-entry URI, or `null` when no matching registration item exists.
- `GetAllVersions()` - Retrieves all versions of a specified package. Returns all versions of the specified package, or null if the package does not exist.
- `GetAllListedVersions()` - Retrieves all listed versions of a package. Returns a list of all listed versions of the specified package.
- `GetLatestListedVersion()` - Returns the highest listed package version, or `null` when none is listed.
- `DeleteAllVersions()` - Unlists all versions of a specified package.
- `Delete()` - Deletes a specific version of a package.
- `GetTransitiveDependencies()` - Recursively resolves, de-duplicates, and caches the transitive package/version list.
- `GetAllPackages()` - Pages through NuGet search and returns all packages owned by the requested owner.

The package also includes 3 additional operations for more specialized cases.
