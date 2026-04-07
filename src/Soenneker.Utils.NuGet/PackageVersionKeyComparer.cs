using System;
using System.Collections.Generic;

namespace Soenneker.Utils.NuGet;

internal sealed class PackageVersionKeyComparer : IEqualityComparer<(string PackageName, string Version)>
{
    internal static readonly PackageVersionKeyComparer Instance = new();

    public bool Equals((string PackageName, string Version) x, (string PackageName, string Version) y) =>
        StringComparer.OrdinalIgnoreCase.Equals(x.PackageName, y.PackageName) && StringComparer.OrdinalIgnoreCase.Equals(x.Version, y.Version);

    public int GetHashCode((string PackageName, string Version) obj)
    {
        unchecked
        {
            int h = StringComparer.OrdinalIgnoreCase.GetHashCode(obj.PackageName);
            h = (h * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Version);
            return h;
        }
    }
}
