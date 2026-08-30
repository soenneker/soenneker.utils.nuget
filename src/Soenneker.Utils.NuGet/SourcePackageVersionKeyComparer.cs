using System;
using System.Collections.Generic;

namespace Soenneker.Utils.NuGet;

internal sealed class SourcePackageVersionKeyComparer : IEqualityComparer<(string Source, string PackageName, string Version)>
{
    internal static readonly SourcePackageVersionKeyComparer Instance = new();

    public bool Equals((string Source, string PackageName, string Version) x, (string Source, string PackageName, string Version) y) =>
        StringComparer.OrdinalIgnoreCase.Equals(x.Source, y.Source) &&
        StringComparer.OrdinalIgnoreCase.Equals(x.PackageName, y.PackageName) &&
        StringComparer.OrdinalIgnoreCase.Equals(x.Version, y.Version);

    public int GetHashCode((string Source, string PackageName, string Version) obj)
    {
        unchecked
        {
            int hash = StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Source);
            hash = (hash * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.PackageName);
            hash = (hash * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Version);
            return hash;
        }
    }
}
